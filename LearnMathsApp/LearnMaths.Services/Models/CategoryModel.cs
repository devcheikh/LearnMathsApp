using LearnMaths.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web;

namespace LearnMaths.Services.Models
{
    [DataContract]
    public class CategoryModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Category, CategoryModel>> FromCategory
        {
            get
            {
                return category => new CategoryModel
                {
                    Name = category.Name
                };
            }
        }
    }
}