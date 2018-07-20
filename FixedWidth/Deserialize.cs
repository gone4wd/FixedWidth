using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mscribel.FixedWidth
{

    public partial class TextSerializer<T> where T : new()
    {

        /// <summary>
        /// Creates T object from fixed width text.
        /// </summary>
        /// <param name="text">string to deserialize</param>
        /// <returns>deserialized object</returns>
        public T Deserialize(string text)
        {

            _currentString = text;
            T deserialized = new T();

            foreach (TextField field in _fields.Values)
            {

                object value = GetObject(field);

                // Set member value
                if (field.Member is FieldInfo)
                {
                    ((FieldInfo)field.Member).SetValue(deserialized, value);
                }
                else if (field.Member is PropertyInfo)
                {
                    ((PropertyInfo)field.Member).SetValue(deserialized, value, null);
                }

            }

            return deserialized;

        }

        /// <summary>
        /// Get T object from string
        /// </summary>
        /// <param name="field">text field</param>
        /// <returns>the T object</returns>
        private object GetObject(TextField field)
        {

            string temp = string.Empty;
            object value = null;

            // Get field text
            int position = ZeroBased == true ? field.Position : field.Position - 1;
            try
            {
                temp = _currentString.Substring(position, field.Size).Trim(field.Padding);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new Exception(string.Format("Position={0}, Size={1}", position, field.Size), e);
            }

            // String to object
            if (field.Formatter != null)
            {
                try
                {
                    value = field.Formatter.Deserialize(temp);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Field: Name={0}, Position={1}, Size={2}",
                        field.Name, field.Position, field.Size), e);
                }
            }
            else
            {
                value = Convert.ChangeType(temp, field.GetMemberType());
            }

            return value;

        }

    }

}