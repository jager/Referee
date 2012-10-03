using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Referee.Models;

namespace Referee.DAL
{
    public class RefereeContext : DbContext
    {
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<RefereeRole> Roles { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<RefClass> RefClasses { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<RefereeEntity> Referees { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Nomination> Nominations { get; set; }
        public DbSet<Nominated> Nominateds { get; set; }
        public DbSet<Voluntary> Voluntaries { get; set; }
        public DbSet<VoluntaryReferee> VoluntaryReferees { get; set; }
        public DbSet<AppConfig> AppConfigs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        

        
    }
}