using CornDome.Models.Cards;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace CornDome.Repository.Cards
{
    public abstract record DqlNode;

    public record DqlCondition(
        string Field,
        string Operator,
        string Value
    ) : DqlNode;

    public record DqlDefaultTerm(string Value) : DqlNode;

    public record DqlLogical(
        string Operator,
        DqlNode Left,
        DqlNode Right
    ) : DqlNode;

    public record DqlNot(DqlNode Operand) : DqlNode;

    public class DqlParser
    {
        private readonly List<string> _tokens = [];
        private int _position;

        private static readonly string[] Operators =
        [
            "&&", "||", ">=", "<=", "!=", "=", ":", ">", "<", "!"
        ];

        public DqlNode Parse(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new FormatException("Search query cannot be empty.");

            _tokens.Clear();
            _position = 0;

            Tokenize(query);

            var result = ParseOr();

            if (_position < _tokens.Count)
                throw Error($"Unexpected token '{Current}'.");

            return result;
        }

        private void Tokenize(string query)
        {
            int i = 0;

            while (i < query.Length)
            {
                if (char.IsWhiteSpace(query[i]))
                {
                    i++;
                    continue;
                }

                if (query[i] == '(' || query[i] == ')')
                {
                    _tokens.Add(query[i].ToString());
                    i++;
                    continue;
                }

                if (query[i] == '"')
                {
                    i++;

                    var value = new StringBuilder();
                    bool closed = false;

                    while (i < query.Length)
                    {
                        if (query[i] == '\\' &&
                            i + 1 < query.Length &&
                            query[i + 1] == '"')
                        {
                            value.Append('"');
                            i += 2;
                            continue;
                        }

                        if (query[i] == '"')
                        {
                            closed = true;
                            i++;
                            break;
                        }

                        value.Append(query[i]);
                        i++;
                    }

                    if (!closed)
                        throw new FormatException("Unclosed quoted string.");

                    _tokens.Add(value.ToString());
                    continue;
                }

                string? op = Operators.FirstOrDefault(
                    o => query.AsSpan(i).StartsWith(o.AsSpan())
                );

                if (op != null)
                {
                    _tokens.Add(op);
                    i += op.Length;
                    continue;
                }

                int start = i;

                while (i < query.Length &&
                       !char.IsWhiteSpace(query[i]) &&
                       query[i] != '(' &&
                       query[i] != ')' &&
                       !Operators.Any(o =>
                           query.AsSpan(i).StartsWith(o.AsSpan())))
                {
                    i++;
                }

                if (start == i)
                    throw new FormatException(
                        $"Unexpected character '{query[i]}'."
                    );

                _tokens.Add(query[start..i]);
            }
        }

        // OR has the lowest precedence.
        private DqlNode ParseOr()
        {
            var left = ParseAnd();

            while (Match("||"))
            {
                var right = ParseAnd();
                left = new DqlLogical("||", left, right);
            }

            return left;
        }

        // AND has higher precedence than OR.
        private DqlNode ParseAnd()
        {
            var left = ParseUnary();

            while (Match("&&"))
            {
                var right = ParseUnary();
                left = new DqlLogical("&&", left, right);
            }

            return left;
        }

        private DqlNode ParseUnary()
        {
            if (Match("!"))
                return new DqlNot(ParseUnary());

            return ParsePrimary();
        }

        private DqlNode ParsePrimary()
        {
            if (Match("("))
            {
                var expression = ParseOr();

                if (!Match(")"))
                    throw Error("Expected closing parenthesis.");

                return expression;
            }

            if (_position >= _tokens.Count)
                throw Error("Expected a search condition.");

            string first = Consume();

            // A field condition has the form field operator value.
            if (_position < _tokens.Count &&
                IsConditionOperator(Current))
            {
                string op = Consume();
                string value = ConsumeMultiwordValue();

                if (string.IsNullOrWhiteSpace(value))
                    throw Error("Expected a value after the operator.");

                return new DqlCondition(first, op, value);
            }

            // Otherwise, search the default text fields.
            return new DqlDefaultTerm(
                $"{first} {ConsumeRemainingWords()}".Trim()
            );
        }

        private string ConsumeMultiwordValue()
        {
            var value = new List<string>();

            while (_position < _tokens.Count &&
                   Current != "&&" &&
                   Current != "||" &&
                   Current != ")")
            {
                value.Add(Consume());
            }

            return string.Join(" ", value);
        }

        private string ConsumeRemainingWords()
        {
            var words = new List<string>();

            while (_position < _tokens.Count &&
                   Current != "&&" &&
                   Current != "||" &&
                   Current != ")")
            {
                words.Add(Consume());
            }

            return string.Join(" ", words);
        }

        private static bool IsConditionOperator(string token)
        {
            return token is ":" or "=" or "!=" or ">" or ">=" or "<" or "<=";
        }

        private bool Match(string token)
        {
            if (_position >= _tokens.Count ||
                _tokens[_position] != token)
                return false;

            _position++;
            return true;
        }

        private string Consume()
        {
            return _tokens[_position++];
        }

        private string Current =>
            _position < _tokens.Count
                ? _tokens[_position]
                : string.Empty;

        private FormatException Error(string message)
        {
            return new FormatException(
                $"{message} (position {_position})."
            );
        }
    }

    public class DqlExpressionBuilder
    {
        private static readonly string[] DefaultSearchFields =
        [
            "name",
            "ability",
            "rulings",
            "landscape",
            "type"
        ];

        public Expression<Func<Card, bool>> Build(DqlNode node)
        {
            var parameter = Expression.Parameter(typeof(Card), "c");

            var body = BuildExpression(node, parameter);

            return Expression.Lambda<Func<Card, bool>>(
                body,
                parameter
            );
        }

        private Expression BuildExpression(
            DqlNode node,
            ParameterExpression parameter)
        {
            return node switch
            {
                DqlCondition condition =>
                    BuildCondition(condition, parameter),

                DqlDefaultTerm term =>
                    BuildDefaultTerm(term, parameter),

                DqlLogical logical =>
                    BuildLogical(logical, parameter),

                DqlNot not =>
                    Expression.Not(
                        BuildExpression(not.Operand, parameter)
                    ),

                _ => throw new NotSupportedException(
                    $"Unsupported query node: {node.GetType().Name}"
                )
            };
        }

        private Expression BuildLogical(
            DqlLogical logical,
            ParameterExpression parameter)
        {
            var left = BuildExpression(logical.Left, parameter);
            var right = BuildExpression(logical.Right, parameter);

            return logical.Operator switch
            {
                "&&" => Expression.AndAlso(left, right),
                "||" => Expression.OrElse(left, right),

                _ => throw new NotSupportedException(
                    $"Unknown logical operator: {logical.Operator}"
                )
            };
        }

        private Expression BuildDefaultTerm(
            DqlDefaultTerm term,
            ParameterExpression parameter)
        {
            // Convenient shorthand.
            if (term.Value.Equals(
                    "custom",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Property(
                    parameter,
                    nameof(Card.IsCustomCard)
                );
            }

            return BuildStringSearch(
                DefaultSearchFields,
                ":",
                term.Value,
                parameter
            );
        }

        private Expression BuildCondition(
            DqlCondition condition,
            ParameterExpression parameter)
        {
            string field = condition.Field.ToLowerInvariant();
            string op = condition.Operator;
            string value = condition.Value;

            switch (field)
            {
                case "name":
                    return BuildStringSearch(
                        ["name"], op, value, parameter);

                case "ability":
                    return BuildStringSearch(
                        ["ability"], op, value, parameter);

                case "rulings":
                    return BuildStringSearch(
                        ["rulings"], op, value, parameter);

                case "type":
                    return BuildStringSearch(
                        ["type"], op, value, parameter);

                case "landscape":
                    return BuildStringSearch(
                        ["landscape"], op, value, parameter);

                case "custom":
                    return BuildBoolean(
                        nameof(Card.IsCustomCard),
                        op,
                        value,
                        parameter);

                case "id":
                    return BuildNumber(
                        nameof(Card.Id),
                        op,
                        value,
                        parameter,
                        nullable: false);

                case "cost":
                    return BuildNumber(
                        nameof(Card.Cost),
                        op,
                        value,
                        parameter,
                        nullable: true);

                case "attack":
                    return BuildNumber(
                        nameof(Card.Attack),
                        op,
                        value,
                        parameter,
                        nullable: true);

                case "defense":
                    return BuildNumber(
                        nameof(Card.Defense),
                        op,
                        value,
                        parameter,
                        nullable: true);

                default:
                    throw new FormatException(
                        $"Unknown search field '{condition.Field}'."
                    );
            }
        }

        private Expression BuildStringSearch(
            string[] fields,
            string op,
            string value,
            ParameterExpression parameter)
        {
            var expressions = fields.Select(field =>
            {
                Expression property = field switch
                {
                    "name" => Expression.Property(
                        parameter, nameof(Card.Name)),

                    "ability" => Expression.Property(
                        parameter, nameof(Card.Ability)),

                    "rulings" => Expression.Property(
                        parameter, nameof(Card.Rulings)),

                    "type" => Expression.Property(
                        Expression.Property(
                            parameter, nameof(Card.CardType)),
                        "Value"),

                    "landscape" => Expression.Property(
                        Expression.Property(
                            parameter, nameof(Card.Landscape)),
                        "Value"),

                    _ => throw new FormatException(
                        $"Unknown text field '{field}'.")
                };

                return BuildStringComparison(property, op, value);
            });

            return expressions.Aggregate(
                Expression.OrElse
            );
        }

        private Expression BuildStringComparison(
            Expression property,
            string op,
            string value)
        {
            // Explicit null checks.
            if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                if (op == ":" || op == "=")
                    return Expression.Equal(
                        property,
                        Expression.Constant(null, property.Type));

                if (op == "!=")
                    return Expression.NotEqual(
                        property,
                        Expression.Constant(null, property.Type));

                throw new FormatException(
                    "Null can only be used with ':' , '=' or '!='."
                );
            }

            if (op == ":" || op == "=" || op == "!=")
            {
                var notNull = Expression.NotEqual(
                    property,
                    Expression.Constant(null, property.Type)
                );

                var lowerProperty = Expression.Call(
                    property,
                    nameof(string.ToLower),
                    Type.EmptyTypes
                );

                var lowerValue = Expression.Constant(
                    value.ToLowerInvariant()
                );

                Expression comparison = op switch
                {
                    ":" => Expression.Call(
                        lowerProperty,
                        nameof(string.Contains),
                        Type.EmptyTypes,
                        lowerValue
                    ),

                    "=" => Expression.Equal(
                        lowerProperty,
                        lowerValue
                    ),

                    "!=" => Expression.NotEqual(
                        lowerProperty,
                        lowerValue
                    ),

                    _ => throw new FormatException(
                        $"Invalid string operator '{op}'.")
                };

                return Expression.AndAlso(notNull, comparison);
            }

            throw new FormatException(
                $"Operator '{op}' is not supported for text fields."
            );
        }

        private Expression BuildNumber(
            string propertyName,
            string op,
            string value,
            ParameterExpression parameter,
            bool nullable)
        {
            var property = Expression.Property(
                parameter,
                propertyName
            );

            // Null checks.
            if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                if (!nullable)
                    throw new FormatException(
                        $"{propertyName} cannot be null.");

                return op switch
                {
                    ":" or "=" => Expression.Equal(
                        property,
                        Expression.Constant(null, property.Type)),

                    "!=" => Expression.NotEqual(
                        property,
                        Expression.Constant(null, property.Type)),

                    _ => throw new FormatException(
                        "Null cannot be used with relational operators.")
                };
            }

            if (!int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int number))
            {
                throw new FormatException(
                    $"'{value}' is not a valid integer for {propertyName}."
                );
            }

            Expression left = nullable
                ? Expression.Call(
                    property,
                    property.Type.GetMethod("GetValueOrDefault", Type.EmptyTypes)!)
                : property;

            var right = Expression.Constant(number);

            return op switch
            {
                ":" or "=" => Expression.Equal(left, right),
                "!=" => Expression.NotEqual(left, right),
                ">" => Expression.GreaterThan(left, right),
                ">=" => Expression.GreaterThanOrEqual(left, right),
                "<" => Expression.LessThan(left, right),
                "<=" => Expression.LessThanOrEqual(left, right),

                _ => throw new FormatException(
                    $"Invalid numeric operator '{op}'.")
            };
        }

        private Expression BuildBoolean(
            string propertyName,
            string op,
            string value,
            ParameterExpression parameter)
        {
            if (!bool.TryParse(value, out bool boolean))
                throw new FormatException(
                    $"'{value}' is not a valid boolean.");

            var property = Expression.Property(
                parameter,
                propertyName
            );

            var constant = Expression.Constant(boolean);

            return op switch
            {
                ":" or "=" => Expression.Equal(property, constant),
                "!=" => Expression.NotEqual(property, constant),

                _ => throw new FormatException(
                    $"Invalid boolean operator '{op}'.")
            };
        }
    }

    public class DweebQueryLanguage
    {
        private readonly DqlParser _parser = new();
        private readonly DqlExpressionBuilder _builder = new();

        public IQueryable<Card> Apply(
            IQueryable<Card> cards,
            string query)
        {
            var parsed = _parser.Parse(query);
            var expression = _builder.Build(parsed);

            return cards.Where(expression);
        }
    }
}
