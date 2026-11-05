
using CornDome.Models.Cards;
using SkiaSharp;

namespace CornDome.Pages.Printer
{
    public interface ICardPdfService
    {
        Task<byte[]> GeneratePdf(IEnumerable<PrintableCard> cards);
    }

    public class PrintableCard
    {
        public required string ImagePath { get; set; }

        public CardTypeEnum CardType { get; set; }
    }

    public class PDFConstants
    {
        // US Letter page dimensions
        public const float PageWidthMm = 215.9f;
        public const float PageHeightMm = 279.4f;

        // Card dimensions
        public const float StandardWidthMm = 63f;
        public const float StandardHeightMm = 88f;

        public const float HeroWidthMm = 76f;
        public const float HeroHeightMm = 101f;

        public const float LandscapeWidthMm = 105f;
        public const float LandscapeHeightMm = 130f;

        // Page layout
        public const float StandardGapMm = 2.5f;
        public const float SpecialGapMm = 3f;
        public const float PageMarginMm = 3.5f;
    }

    public class PDFCreator(Config config) : ICardPdfService
    {
        // Image quality / rendering
        private static readonly SKSamplingOptions Sampling =
            new(SKFilterMode.Linear, SKMipmapMode.Linear);

        private static float MmToPoints(float mm)
        {
            return mm * 72f / 25.4f;
        }

        private record CardSize(float Width, float Height);

        private record CardPlacement(
            PrintableCard Card,
            float X,
            float Y,
            CardSize Size);

        private record CardRow(
            List<CardPlacement> Cards,
            float Height,
            float Width);

        private string GetImageFullPath(string cardImagePath) => Path.Combine(config.AppData.ImagePath, cardImagePath);

        public Task<byte[]> GeneratePdf(IEnumerable<PrintableCard> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);

            var cardList = cards.ToList();

            if (cardList.Count == 0)
                throw new ArgumentException("No cards were provided.", nameof(cards));

            foreach (var card in cardList)
            {
                if (string.IsNullOrWhiteSpace(card.ImagePath))
                    throw new ArgumentException("A card has an empty image path.");

                if (!File.Exists(GetImageFullPath(card.ImagePath)))
                    throw new FileNotFoundException(
                        $"Card image not found: {card.ImagePath}",
                        card.ImagePath);
            }

            return Task.FromResult(CreatePdf(cardList));
        }

        private byte[] CreatePdf(List<PrintableCard> cards)
        {
            using var stream = new MemoryStream();

            using var document = SKDocument.CreatePdf(stream);

            float pageWidth = MmToPoints(PDFConstants.PageWidthMm);
            float pageHeight = MmToPoints(PDFConstants.PageHeightMm);

            // Hero and Landscape cards are printed first.
            var specialCards = cards
                .Where(c => c.CardType == CardTypeEnum.Hero ||
                            c.CardType == CardTypeEnum.Landscape)
                .ToList();

            // Everything else is treated as a standard card.
            var standardCards = cards
                .Where(c => c.CardType != CardTypeEnum.Hero &&
                            c.CardType != CardTypeEnum.Landscape)
                .ToList();

            // Generate pages containing Hero and Landscape cards.
            if (specialCards.Count > 0)
            {
                var specialPages = CreateSpecialCardPages(specialCards);

                foreach (var placements in specialPages)
                {
                    using var canvas = document.BeginPage(
                        pageWidth,
                        pageHeight);

                    DrawCards(canvas, placements);
                    document.EndPage();
                }
            }

            // Generate standard card pages in 3x3 grids.
            if (standardCards.Count > 0)
            {
                var standardPages = CreateStandardCardPages(standardCards);

                foreach (var placements in standardPages)
                {
                    using var canvas = document.BeginPage(
                        pageWidth,
                        pageHeight);

                    DrawCards(canvas, placements);
                    document.EndPage();
                }
            }

            document.Close();

            return stream.ToArray();
        }

        private static CardSize GetCardSize(PrintableCard card)
        {
            return card.CardType switch
            {
                CardTypeEnum.Hero =>
                    new CardSize(PDFConstants.HeroWidthMm, PDFConstants.HeroHeightMm),

                CardTypeEnum.Landscape =>
                    new CardSize(PDFConstants.LandscapeWidthMm, PDFConstants.LandscapeHeightMm),

                _ =>
                    new CardSize(PDFConstants.StandardWidthMm, PDFConstants.StandardHeightMm)
            };
        }

        #region Special Card Layout

        private static List<List<CardPlacement>> CreateSpecialCardPages(
            List<PrintableCard> cards)
        {
            var pages = new List<List<CardPlacement>>();

            float margin = MmToPoints(PDFConstants.PageMarginMm);
            float gap = MmToPoints(PDFConstants.SpecialGapMm);

            float availableWidth =
                MmToPoints(PDFConstants.PageWidthMm) - (margin * 2);

            float availableHeight =
                MmToPoints(PDFConstants.PageHeightMm) - (margin * 2);

            // Sort larger cards first to make the row-packing
            // algorithm more efficient.
            var sortedCards = cards
                .Select(card => new
                {
                    Card = card,
                    Size = GetCardSize(card)
                })
                .OrderByDescending(x => x.Size.Height)
                .ThenByDescending(x => x.Size.Width)
                .ToList();

            var currentPage = new List<CardPlacement>();

            float currentY = margin;
            float currentX = margin;

            float currentRowHeight = 0;

            foreach (var item in sortedCards)
            {
                float cardWidth = MmToPoints(item.Size.Width);
                float cardHeight = MmToPoints(item.Size.Height);

                // Start a new row if this card doesn't fit horizontally.
                if (currentX + cardWidth > margin + availableWidth)
                {
                    currentY += currentRowHeight + gap;
                    currentX = margin;
                    currentRowHeight = 0;
                }

                // Start a new page if this card doesn't fit vertically.
                if (currentY + cardHeight > margin + availableHeight)
                {
                    if (currentPage.Count > 0)
                        pages.Add(currentPage);

                    currentPage = new List<CardPlacement>();

                    currentX = margin;
                    currentY = margin;
                    currentRowHeight = 0;
                }

                currentPage.Add(new CardPlacement(
                    item.Card,
                    currentX,
                    currentY,
                    item.Size));

                currentX += cardWidth + gap;

                currentRowHeight = Math.Max(
                    currentRowHeight,
                    cardHeight);
            }

            if (currentPage.Count > 0)
                pages.Add(currentPage);

            return pages;
        }

        #endregion

        #region Standard Card Layout

        private static List<List<CardPlacement>> CreateStandardCardPages(
            List<PrintableCard> cards)
        {
            var pages = new List<List<CardPlacement>>();

            float cardWidth = MmToPoints(PDFConstants.StandardWidthMm);
            float cardHeight = MmToPoints(PDFConstants.StandardHeightMm);

            float gap = MmToPoints(PDFConstants.StandardGapMm);

            float pageWidth = MmToPoints(PDFConstants.PageWidthMm);
            float pageHeight = MmToPoints(PDFConstants.PageHeightMm);

            // Center the entire 3x3 grid on the page.
            float gridWidth = (cardWidth * 3) + (gap * 2);
            float gridHeight = (cardHeight * 3) + (gap * 2);

            float startX = (pageWidth - gridWidth) / 2;
            float startY = (pageHeight - gridHeight) / 2;

            // Nine cards per page.
            for (int i = 0; i < cards.Count; i += 9)
            {
                var pageCards = new List<CardPlacement>();

                var pageBatch = cards
                    .Skip(i)
                    .Take(9)
                    .ToList();

                for (int j = 0; j < pageBatch.Count; j++)
                {
                    int row = j / 3;
                    int column = j % 3;

                    float x = startX + column * (cardWidth + gap);
                    float y = startY + row * (cardHeight + gap);

                    pageCards.Add(new CardPlacement(
                        pageBatch[j],
                        x,
                        y,
                        new CardSize(
                            PDFConstants.StandardWidthMm,
                            PDFConstants.StandardHeightMm)));
                }

                pages.Add(pageCards);
            }

            return pages;
        }

        #endregion

        #region Drawing

        private void DrawCards(
            SKCanvas canvas,
            List<CardPlacement> placements)
        {
            using var borderPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 0.4f,
                IsAntialias = true
            };

            foreach (var placement in placements)
            {
                var path = GetImageFullPath(placement.Card.ImagePath);
                using var bitmap = SKBitmap.Decode(
                    path)
                    ?? throw new InvalidOperationException(
                        $"Unable to decode image: {placement.Card.ImagePath}");

                using var image = SKImage.FromBitmap(bitmap);

                float width = MmToPoints(placement.Size.Width);
                float height = MmToPoints(placement.Size.Height);

                var destination = new SKRect(
                    placement.X,
                    placement.Y,
                    placement.X + width,
                    placement.Y + height);

                // Draw the complete image while preserving aspect ratio.
                float sourceRatio = (float)bitmap.Width / bitmap.Height;
                float destinationRatio = width / height;

                float drawWidth = width;
                float drawHeight = height;

                // KEEP THIS in case we want to do scaling
                //if (sourceRatio > destinationRatio)
                //{
                //    drawHeight = width / sourceRatio;
                //}
                //else
                //{
                //    drawWidth = height * sourceRatio;
                //}

                float offsetX = (width - drawWidth) / 2;
                float offsetY = (height - drawHeight) / 2;

                var imageRect = new SKRect(
                    placement.X + offsetX,
                    placement.Y + offsetY,
                    placement.X + offsetX + drawWidth,
                    placement.Y + offsetY + drawHeight);

                canvas.DrawImage(
                    image,
                    imageRect,
                    Sampling);

                // Draw the physical card boundary.
                // canvas.DrawRect(destination, borderPaint);
            }
        }

        #endregion
    }
}
