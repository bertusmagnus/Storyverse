/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    public interface IParentEntity
    {
        void AddChild<T>(T item) where T : IEntity;
        void RemoveChild<T>(T item) where T : IEntity;
        void RemoveChild<T>(Guid id) where T : IEntity;
    }
}