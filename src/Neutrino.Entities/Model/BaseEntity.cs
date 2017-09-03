using System;

namespace Neutrino.Entities.Model
{
    public class BaseEntity
    {
        public virtual string Id { get; set; }

        public virtual DateTime CreatedDate { get; set; }
    }
}