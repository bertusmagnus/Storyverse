/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    public class EntityUtility
    {
        public static T[] CollectionToArray<T>(ICollection<T> collection)
        {
            T[] result = new T[collection.Count];
            collection.CopyTo(result, 0);
            return result;
        }

        public static List<string> ValidateCollection<T>(IList<T> collection)
            where T : IEntity
        {
            List<string> messages = new List<string>();
            foreach (T item in collection)
            {
                try
                {
                    item.Validate();
                }
                catch (ValidationException ex)
                {
                    foreach (string message in ex.ValidationErrorMessages)
                    {
                        messages.Add(message);
                    }
                }
            }
            return messages;
        }

        public static ValidationException BuildValidationException(List<string> messages)
        {
            string[] messagesArray = new string[messages.Count];
            messages.CopyTo(messagesArray, 0);
            throw new ValidationException("The entity is not valid", messagesArray);
        }

        //public static void AddChild<T>(IEntity entity, T item)
        //    where T : IEntity
        //{
        //    IList<T> list = GetCollection<T>(entity);
        //    if (list != null)
        //    {
        //        if (!list.Contains(item))
        //        {
        //            list.Add(item);
        //        }
        //    }
        //}

        //public static void RemoveChild<T>(IEntity entity, T item)
        //    where T : IEntity
        //{
        //    IList<T> list = GetCollection<T>(entity);
        //    if (list != null)
        //    {
        //        if (list.Contains(item))
        //        {
        //            list.Remove(item);
        //        }
        //    }
        //}

        //public static void RemoveChild<T>(IEntity entity, Guid id, bool inverse)
        //    where T : IEntity
        //{
        //    IList<T> list = GetCollection<T>(entity);
        //    foreach (T item in list)
        //    {
        //        if (item.Id == id)
        //        {
        //            if (list.Contains(item))
        //            {
        //                list.Remove(item);
        //            }
        //            break;
        //        }
        //    }
        //}

        //private static IList<T> GetCollection<T>(IEntity entity)
        //    where T : IEntity
        //{
        //    IList<T> list = null;
        //    foreach (PropertyInfo prop in entity.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
        //    {
        //        Type type = prop.PropertyType;
        //        if (type.IsGenericType && type.GetGenericArguments()[0] == typeof(T) && prop.CanWrite)
        //        {
        //            list = (IList<T>)prop.GetValue(entity, null);
        //        }
        //    }
        //    return list;
        //}

    }
}