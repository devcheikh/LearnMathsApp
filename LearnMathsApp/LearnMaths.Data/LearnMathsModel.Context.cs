﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LearnMaths.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db95e1649d89d04f53bfe6a26f0095fb22Entities : DbContext
    {
        public db95e1649d89d04f53bfe6a26f0095fb22Entities()
            : base("name=db95e1649d89d04f53bfe6a26f0095fb22Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CoverLevel> CoverLevels { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}