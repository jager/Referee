using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;

namespace Referee.DAL
{
    public class UOW : IDisposable
    {
        protected RefereeContext db = new RefereeContext();
        private bool disposed;


        #region RepositoryDefinitions;

        private GenericRepository<Club> clubRepository;
        public GenericRepository<Club> ClubRepository
        {
            get
            {
                if (this.clubRepository == null)
                {
                    this.clubRepository = new GenericRepository<Club>(db);
                }
                return this.clubRepository;
            }
        }

        private GenericRepository<Team> teamRepository;
        public GenericRepository<Team> TeamRepository
        {
            get
            {
                if (this.teamRepository == null)
                {
                    this.teamRepository = new GenericRepository<Team>(db);
                }
                return this.teamRepository;
            }
        }

        private GenericRepository<Season> seasonRepository;
        public GenericRepository<Season> SeasonRepository
        {
            get
            {
                if (this.seasonRepository == null)
                {
                    this.seasonRepository = new GenericRepository<Season>(db);
                }
                return this.seasonRepository;
            }
        }


        private GenericRepository<Game> gameRepository;
        public GenericRepository<Game> GameRepository
        {
            get
            {
                if (this.gameRepository == null)
                {
                    this.gameRepository = new GenericRepository<Game>(db);
                }
                return this.gameRepository;
            }
        }

        private GenericRepository<RefereeEntity> refereeRepository;
        public GenericRepository<RefereeEntity> RefereeRepository
        {
            get
            {
                if (this.refereeRepository == null)
                {
                    this.refereeRepository = new GenericRepository<RefereeEntity>(db);
                }
                return this.refereeRepository;
            }
        }

        private GenericRepository<League> leagueRepository;
        public GenericRepository<League> LeagueRepository
        {
            get
            {
                if (this.leagueRepository == null)
                {
                    this.leagueRepository = new GenericRepository<League>(db);
                }
                return this.leagueRepository;
            }
        }

        private GenericRepository<Enrollment> enrollmentRepository;
        public GenericRepository<Enrollment> EnrollmentRepository
        {
            get
            {
                if (this.enrollmentRepository == null)
                {
                    this.enrollmentRepository = new GenericRepository<Enrollment>(db);
                }
                return this.enrollmentRepository;
            }
        }

        private GenericRepository<RefClass> rclassRepository;
        public GenericRepository<RefClass> RClassRepository
        {
            get
            {
                if (this.rclassRepository == null)
                {
                    this.rclassRepository = new GenericRepository<RefClass>(db);
                }
                return this.rclassRepository;
            }
        }

        private GenericRepository<Authorization> authorizationRepository;
        public GenericRepository<Authorization> AuthorizationRepository
        {
            get
            {
                if (this.authorizationRepository == null)
                {
                    this.authorizationRepository = new GenericRepository<Authorization>(db);
                }
                return this.authorizationRepository;
            }
        }

        private GenericRepository<Availability> availabilityRepository;
        public GenericRepository<Availability> AvailabilityRepository
        {
            get
            {
                if (this.availabilityRepository == null)
                {
                    this.availabilityRepository = new GenericRepository<Availability>(db);
                }
                return this.availabilityRepository;
            }
        }

        private GenericRepository<Tournament> tournamentReposiotry;
        public GenericRepository<Tournament> TournamentRepository
        {
            get 
            {
                if (this.tournamentReposiotry == null)
                {
                    this.tournamentReposiotry = new GenericRepository<Tournament>(db);
                }
                return this.tournamentReposiotry;
            }
        }

        private GenericRepository<Nomination> nominationRepository;
        public GenericRepository<Nomination> NominationRepository
        {
            get
            {
                if (this.nominationRepository == null)
                {
                    this.nominationRepository = new GenericRepository<Nomination>(db);
                }
                return this.nominationRepository;
            }
        }

        private GenericRepository<Nominated> nominatedRepository;
        public GenericRepository<Nominated> NominatedRepository
        {
            get
            {
                if (this.nominatedRepository == null)
                {
                    this.nominatedRepository = new GenericRepository<Nominated>(db);
                }
                return this.nominatedRepository;
            }
        }

        private GenericRepository<Function> functionRepository;
        public GenericRepository<Function> FunctionRepository
        {
            get
            {
                if (this.functionRepository == null)
                {
                    this.functionRepository = new GenericRepository<Function>(db);
                }
                return this.functionRepository;
            }
        }

        private GenericRepository<RefereeRole> refRoleRepository;
        public GenericRepository<RefereeRole> RefRoleRepository
        {
            get
            {
                if (this.refRoleRepository == null)
                {
                    this.refRoleRepository = new GenericRepository<RefereeRole>(db);
                }
                return this.refRoleRepository;
            }
        }

        private GenericRepository<Voluntary> voluntaryRepository;
        public GenericRepository<Voluntary> VoluntaryRepository
        {
            get
            {
                if (this.voluntaryRepository == null)
                {
                    this.voluntaryRepository = new GenericRepository<Voluntary>(db);
                }
                return this.voluntaryRepository;
            }
        }

        private GenericRepository<VoluntaryReferee> vRefereeRepository;
        public GenericRepository<VoluntaryReferee> VRefereeRepository
        {
            get
            {
                if (this.vRefereeRepository == null)
                {
                    this.vRefereeRepository = new GenericRepository<VoluntaryReferee>(db);
                }
                return this.vRefereeRepository;
            }
        }

        #endregion;


        public void Save()
        {
            db.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}