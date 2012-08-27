using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Referee.Models;
using Referee.DAL;
using Referee.ViewModels;

namespace Referee.Repositories
{
    public class GamesNominatedRepository : UOW
    {
        private Guid _refId;
        public Guid RefereeId
        {
            get
            {
                return this._refId;
            }
        }


        public GamesNominatedRepository(Guid refereeId)
        {
            this._refId = refereeId;
        }

        public IEnumerable<Nominated> Get()
        {


            //var Nom = (from n in db.Nominateds where n.RefereeId.Equals(RefereeId) select n);
            /*var query = (from nd in db.Nominateds
                    from no in db.Nominations.Where(nom => nom.Id == nd.NominationId).DefaultIfEmpty()
                    from gm in db.Games.Where(g => g.Id ==no.GameId).DefaultIfEmpty()
                    where nd.RefereeId.Equals(RefereeId)
                    select new 
                    {
                        Game = gm,
                        Nomination = no
                    }).ToList();   
            return query;
            return (from q in query.AsEnumerable()
                    select new GameNomination { Game = q.Game, Nomination = q.Nomination }).ToList<GameNomination>();
            IQueryable<Nominated> query = db.Set<Nominated>();
            query = query.Where(n => n.RefereeId.Equals(RefereeId));
            return (query.Include("Nomination").Include("Game")).toList();*/
            ///var RefereeNominations = db.Nominateds;//.Where(n => n.RefereeId.Equals(RefereeId)).Select(n1 => n1.NominationId);
            ///

            return NominatedRepository.Get();


                /*

            if (RefereeNominations.Count() > 0)
            {
                return (this.NominationRepository.Get(filter: n => RefereeNominations.Contains(n.Id)));
            }
            return new List<Nomination>();
            */
        }
    }
}