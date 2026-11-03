
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornDome.Repository.Migrations
{
    public partial class MergeCardAndCardRevision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                -- 1. Identify the latest revision for every card.
                CREATE TEMP TABLE _LatestRevision AS
                SELECT
                    Id,
                    CardId,
                    RevisionNumber,
                    Name,
                    TypeId,
                    Ability,
                    LandscapeId,
                    Cost,
                    Attack,
                    Defense,
                    Rulings
                FROM (
                    SELECT
                        r.*,
                        ROW_NUMBER() OVER (
                            PARTITION BY r.CardId
                            ORDER BY r.RevisionNumber DESC, r.Id DESC
                        ) AS RowNum
                    FROM revision r
                )
                WHERE RowNum = 1;

                -- 2. Preserve images belonging to the latest revision.
                CREATE TEMP TABLE _LatestCardImages AS
                SELECT
                    ci.Id,
                    lr.CardId,
                    ci.CardImageTypeId,
                    ci.ImageUrl
                FROM cardImage ci
                INNER JOIN _LatestRevision lr
                    ON lr.Id = ci.RevisionId;

                -- 3. Preserve set associations belonging to the latest revision.
                CREATE TEMP TABLE _LatestCardSets AS
                SELECT DISTINCT
                    lr.CardId AS CardsId,
                    rs.CardSetsId
                FROM revisionSet rs
                INNER JOIN _LatestRevision lr
                    ON lr.Id = rs.RevisionsId;

                -- 4. Remove old tables that reference revisions.
                DROP TABLE cardImage;
                DROP TABLE revisionSet;
                DROP TABLE revision;

                -- 5. Create the new card table with all revision properties.
                CREATE TABLE card_new (
                    Id INTEGER NOT NULL PRIMARY KEY,
                    IsCustomCard INTEGER NOT NULL DEFAULT 0,
                    Name TEXT NULL,
                    TypeId INTEGER NULL,
                    Ability TEXT NULL,
                    LandscapeId INTEGER NULL,
                    Cost INTEGER NULL,
                    Attack INTEGER NULL,
                    Defense INTEGER NULL,
                    Rulings TEXT NULL,

                    CONSTRAINT FK_card_cardType_TypeId
                        FOREIGN KEY (TypeId)
                        REFERENCES cardType (Id)
                        ON DELETE SET NULL,

                    CONSTRAINT FK_card_landscape_LandscapeId
                        FOREIGN KEY (LandscapeId)
                        REFERENCES landscape (Id)
                        ON DELETE SET NULL
                );

                -- 6. Copy each card and its latest revision data.
                INSERT INTO card_new (
                    Id,
                    IsCustomCard,
                    Name,
                    TypeId,
                    Ability,
                    LandscapeId,
                    Cost,
                    Attack,
                    Defense,
                    Rulings
                )
                SELECT
                    c.Id,
                    c.IsCustomCard,
                    lr.Name,
                    lr.TypeId,
                    lr.Ability,
                    lr.LandscapeId,
                    lr.Cost,
                    lr.Attack,
                    lr.Defense,
                    lr.Rulings
                FROM card c
                INNER JOIN _LatestRevision lr
                    ON lr.CardId = c.Id;

                -- 7. Replace the original card table.
                DROP TABLE card;
                ALTER TABLE card_new RENAME TO card;

                -- 8. Recreate cardImage with CardId instead of RevisionId.
                CREATE TABLE cardImage (
                    Id INTEGER NOT NULL PRIMARY KEY,
                    CardId INTEGER NOT NULL,
                    CardImageTypeId INTEGER NOT NULL,
                    ImageUrl TEXT NULL,

                    CONSTRAINT FK_cardImage_card_CardId
                        FOREIGN KEY (CardId)
                        REFERENCES card (Id)
                        ON DELETE CASCADE,

                    CONSTRAINT FK_cardImage_cardImageType_CardImageTypeId
                        FOREIGN KEY (CardImageTypeId)
                        REFERENCES cardImageType (Id)
                        ON DELETE CASCADE
                );

                INSERT INTO cardImage (
                    Id,
                    CardId,
                    CardImageTypeId,
                    ImageUrl
                )
                SELECT
                    Id,
                    CardId,
                    CardImageTypeId,
                    ImageUrl
                FROM _LatestCardImages;

                -- 9. Create the new card-to-set relationship.
                CREATE TABLE cardSet (
                    CardSetsId INTEGER NOT NULL,
                    CardsId INTEGER NOT NULL,

                    CONSTRAINT PK_cardSet
                        PRIMARY KEY (CardSetsId, CardsId),

                    CONSTRAINT FK_cardSet_set_CardSetsId
                        FOREIGN KEY (CardSetsId)
                        REFERENCES "set" (Id)
                        ON DELETE CASCADE,

                    CONSTRAINT FK_cardSet_card_CardsId
                        FOREIGN KEY (CardsId)
                        REFERENCES card (Id)
                        ON DELETE CASCADE
                );

                INSERT INTO cardSet (CardSetsId, CardsId)
                SELECT CardSetsId, CardsId
                FROM _LatestCardSets;

                -- 10. Recreate indexes.
                CREATE INDEX IX_card_TypeId
                    ON card (TypeId);

                CREATE INDEX IX_card_LandscapeId
                    ON card (LandscapeId);

                CREATE INDEX IX_cardImage_CardId
                    ON cardImage (CardId);

                CREATE INDEX IX_cardImage_CardImageTypeId
                    ON cardImage (CardImageTypeId);

                CREATE INDEX IX_cardSet_CardsId
                    ON cardSet (CardsId);

                -- 11. Remove temporary migration data.
                DROP TABLE _LatestRevision;
                DROP TABLE _LatestCardImages;
                DROP TABLE _LatestCardSets;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "This migration is irreversible because historical card revisions were discarded.");
        }
    }
}
