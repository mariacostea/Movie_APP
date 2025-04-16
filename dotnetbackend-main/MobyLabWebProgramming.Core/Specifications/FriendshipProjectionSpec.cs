using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications
{
    public sealed class FriendshipProjectionSpec : Specification<Friendship, FriendshipDTO>
    {
        public FriendshipProjectionSpec(bool orderByCreatedAt = false)
        {
            Query
                .Include(f => f.Requester)
                .Include(f => f.Addressee);
            
            Query.Select(f => new FriendshipDTO
            {
                Id = f.Id,
                RequesterId = f.RequesterId,
                RequesterName = f.Requester != null ? f.Requester.Name : null,
                AddresseeId = f.AddresseeId,
                AddresseeName = f.Addressee != null ? f.Addressee.Name : null,
                Status = f.Status,
                RequestedAt = f.RequestedAt,
                AcceptedAt = f.AcceptedAt
            });

            if (orderByCreatedAt)
            {
                Query.OrderByDescending(x => x.CreatedAt);
            }
        }
        
        public FriendshipProjectionSpec(Guid id) : this()
        {
            Query.Where(f => f.Id == id);
        }

        public FriendshipProjectionSpec(Guid userId, bool filterByUser) : this()
        {
            Query.Where(f => f.RequesterId == userId || f.AddresseeId == userId);
        }
        
        public FriendshipProjectionSpec(string status) : this(true)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                Query.Where(f => f.Status == status);
            }
        }
        
        public FriendshipProjectionSpec(string? search, bool searchByName) : this(true)
        {
            search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;
            if (search == null) return;

            var expr = $"%{search.Replace(" ", "%")}%";
            
            Query.Where(f =>
                EF.Functions.Like(f.Requester.Name, expr) ||
                EF.Functions.Like(f.Addressee.Name, expr));
        }
    }
}
