using CornDome.Models.Main;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Cards
{
    public interface ICustomCardRepository
    {
        Task<CustomCard> Add(CustomCard customCard);
        Task<CustomCard> GetByImageUrl(string imageUrl);
        Task<CustomCard> GetById(Guid id);
        Task<int> CountByUserId(int userId);
        Task<List<CustomCard>> GetByUserId(int userId);
        Task<bool> UpdateVisibility(Guid id, int userId, Visibility visibility);
        Task<bool> Delete(Guid id, int userId);
        Task<List<CustomCard>> GetUnapproved();
        Task<bool> Approve(Guid id, int adminUserId);
        Task<bool> Deny(Guid id);
        Task<List<CustomCard>> GetAllPublic();
        Task<List<CustomCard>> GetAll();
        Task<bool> Update(CustomCard customCard);
        Task<bool> Delete(Guid id);
    }

    public class CustomCardRepository(MainContext mainContext) : ICustomCardRepository
    {
        public async Task<CustomCard> Add(CustomCard customCard)
        {
            mainContext.CustomCards.Add(customCard);
            await mainContext.SaveChangesAsync();
            return customCard;
        }

        public async Task<CustomCard> GetById(Guid id)
        {
            return await mainContext.CustomCards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomCard> GetByImageUrl(string imageUrl)
        {
            return await mainContext.CustomCards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ImageUrl == imageUrl);
        }

        public async Task<int> CountByUserId(int userId)
        {
            return await mainContext.CustomCards.CountAsync(x => x.UserId == userId);
        }

        public async Task<List<CustomCard>> GetByUserId(int userId)
        {
            return await mainContext.CustomCards
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateVisibility(Guid id, int userId, Visibility visibility)
        {
            var card = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (card == null)
                return false;

            card.Visibility = visibility;
            card.UpdatedAt = DateTime.UtcNow;

            await mainContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id, int userId)
        {
            var card = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (card == null)
                return false;

            mainContext.CustomCards.Remove(card);
            await mainContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<CustomCard>> GetUnapproved()
        {
            return await mainContext.CustomCards
                .AsNoTracking()
                .Where(x => x.ApprovedBy == null)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> Approve(Guid id, int adminUserId)
        {
            var card = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == id && x.ApprovedBy == null);

            if (card == null)
                return false;

            card.ApprovedBy = adminUserId;
            card.UpdatedAt = DateTime.UtcNow;

            await mainContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Deny(Guid id)
        {
            var card = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == id && x.ApprovedBy == null);

            if (card == null)
                return false;

            card.ApprovedBy = 0;
            card.UpdatedAt = DateTime.UtcNow;

            await mainContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<CustomCard>> GetAllPublic()
        {
            var cards = await mainContext.CustomCards
                .AsNoTracking()
                .Where(x => x.ApprovedBy != null && x.ApprovedBy > 0 && x.Visibility == Visibility.Visible)
                .ToListAsync();

            return cards;
        }

        public async Task<List<CustomCard>> GetAll()
        {
            return await mainContext.CustomCards
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> Update(CustomCard customCard)
        {
            var existing = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == customCard.Id);

            if (existing == null)
                return false;

            existing.Name = customCard.Name;
            existing.CardType = customCard.CardType;
            existing.Landscape = customCard.Landscape;
            existing.Ability = customCard.Ability;
            existing.Cost = customCard.Cost;
            existing.Attack = customCard.Attack;
            existing.Defense = customCard.Defense;
            existing.Author = customCard.Author;
            existing.Artist = customCard.Artist;
            existing.ImageUrl = customCard.ImageUrl;
            existing.Visibility = customCard.Visibility;
            existing.ApprovedBy = customCard.ApprovedBy;
            existing.UpdatedAt = DateTime.UtcNow;

            await mainContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var card = await mainContext.CustomCards
                .FirstOrDefaultAsync(x => x.Id == id);

            if (card == null)
                return false;

            mainContext.CustomCards.Remove(card);
            await mainContext.SaveChangesAsync();
            return true;
        }
    }
}