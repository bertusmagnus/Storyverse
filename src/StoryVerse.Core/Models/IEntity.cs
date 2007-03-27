using System;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    public interface IEntity
    {
        Guid Id { get; }
        void Validate();
        void Refresh();
        void CreateAndFlush();
        void UpdateAndFlush();
        void DeleteAndFlush();
    }
}