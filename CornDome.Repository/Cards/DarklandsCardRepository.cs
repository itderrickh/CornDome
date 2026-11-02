using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CornDome.Repository.Cards
{
    public interface IDarklandsCardRepository
    {
        // Darklands Cards
        Task<DarklandsCard?> GetDarklandsCard(int id);
        Task<IList<DarklandsCard>> GetDarklandsCards();
        Task<DarklandsCard> AddDarklandsCard(DarklandsCard card);
        Task<DarklandsCard> EditDarklandsCard(DarklandsCard card);
        Task<bool> DeleteDarklandsCard(int id);

        // Darklands Card Types
        Task<DarklandsCardType?> GetDarklandsCardType(int id);
        Task<IList<DarklandsCardType>> GetDarklandsCardTypes();
        Task<DarklandsCardType> AddDarklandsCardType(DarklandsCardType cardType);
        Task<DarklandsCardType> EditDarklandsCardType(DarklandsCardType cardType);
        Task<bool> DeleteDarklandsCardType(int id);

        // Darklands Overlords
        Task<DarklandsOverlord?> GetDarklandsOverlord(int id);
        Task<IList<DarklandsOverlord>> GetDarklandsOverlords();
        Task<DarklandsOverlord> AddDarklandsOverlord(DarklandsOverlord overlord);
        Task<DarklandsOverlord> EditDarklandsOverlord(DarklandsOverlord overlord);
        Task<bool> DeleteDarklandsOverlord(int id);

        Task AddDarklandsCardImage(DarklandsCardImage image);
        Task DeleteDarklandsCardImages(int cardId);
    }

    public class DarklandsCardRepository(CardDatabaseContext context) : IDarklandsCardRepository
    {

        // Darklands Cards

        public async Task<DarklandsCard?> GetDarklandsCard(int id)
        {
            return await context.DarklandsCards
                .Include(x => x.CardType)
                .Include(x => x.Overlord)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<DarklandsCard>> GetDarklandsCards()
        {
            return await context.DarklandsCards
                .Include(x => x.CardType)
                .Include(x => x.Overlord)
                .Include(x => x.Images)
                    .ThenInclude(x => x.CardImageType)
                .ToListAsync();
        }

        public async Task<DarklandsCard> AddDarklandsCard(DarklandsCard card)
        {
            context.DarklandsCards.Add(card);
            await context.SaveChangesAsync();

            return card;
        }

        public async Task<DarklandsCard> EditDarklandsCard(DarklandsCard card)
        {
            context.DarklandsCards.Update(card);
            await context.SaveChangesAsync();

            return card;
        }

        public async Task<bool> DeleteDarklandsCard(int id)
        {
            var card = await context.DarklandsCards.FindAsync(id);

            if (card == null)
                return false;

            context.DarklandsCards.Remove(card);
            await context.SaveChangesAsync();

            return true;
        }

        // Darklands Card Types

        public async Task<DarklandsCardType?> GetDarklandsCardType(int id)
        {
            return await context.DarklandsCardTypes
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<DarklandsCardType>> GetDarklandsCardTypes()
        {
            return await context.DarklandsCardTypes.ToListAsync();
        }

        public async Task<DarklandsCardType> AddDarklandsCardType(DarklandsCardType cardType)
        {
            context.DarklandsCardTypes.Add(cardType);
            await context.SaveChangesAsync();

            return cardType;
        }

        public async Task<DarklandsCardType> EditDarklandsCardType(DarklandsCardType cardType)
        {
            context.DarklandsCardTypes.Update(cardType);
            await context.SaveChangesAsync();

            return cardType;
        }

        public async Task<bool> DeleteDarklandsCardType(int id)
        {
            var cardType = await context.DarklandsCardTypes.FindAsync(id);

            if (cardType == null)
                return false;

            context.DarklandsCardTypes.Remove(cardType);
            await context.SaveChangesAsync();

            return true;
        }

        // Darklands Overlords

        public async Task<DarklandsOverlord?> GetDarklandsOverlord(int id)
        {
            return await context.DarklandsOverlords
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<DarklandsOverlord>> GetDarklandsOverlords()
        {
            return await context.DarklandsOverlords.ToListAsync();
        }

        public async Task<DarklandsOverlord> AddDarklandsOverlord(DarklandsOverlord overlord)
        {
            context.DarklandsOverlords.Add(overlord);
            await context.SaveChangesAsync();

            return overlord;
        }

        public async Task<DarklandsOverlord> EditDarklandsOverlord(DarklandsOverlord overlord)
        {
            context.DarklandsOverlords.Update(overlord);
            await context.SaveChangesAsync();

            return overlord;
        }

        public async Task<bool> DeleteDarklandsOverlord(int id)
        {
            var overlord = await context.DarklandsOverlords.FindAsync(id);

            if (overlord == null)
                return false;

            context.DarklandsOverlords.Remove(overlord);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task AddDarklandsCardImage(DarklandsCardImage image)
        {
            context.DarklandsCardImages.Add(image);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDarklandsCardImages(int cardId)
        {
            var images = await context.DarklandsCardImages
                .Where(x => x.CardId == cardId)
                .ToListAsync();

            context.DarklandsCardImages.RemoveRange(images);

            await context.SaveChangesAsync();
        }
    }
}
