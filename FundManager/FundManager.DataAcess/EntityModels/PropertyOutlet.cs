
using FundManager.Common.BaseEntity;

namespace FundManager.DataAccess.EntityModels
{
    /// <summary>
    /// Join table for many-to-many relationship between Property and Outlet.
    /// One outlet can belong to many properties; one property can have many outlets.
    /// </summary>
    public class PropertyOutlet : BaseEntity
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int OutletId { get; set; }
        public Property? Property { get; set; }
        public Outlet? Outlet { get; set; }
    }
}