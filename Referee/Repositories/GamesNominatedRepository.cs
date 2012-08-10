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

        public List<GameNomination> Get()
        {
            return (from n in db.Nominateds
                    from nom in db.Nominations
                    from g in db.Games
                    where n.NominationId == nom.Id.ToString() &&
                            nom.GameId == g.Id &&  
                            n.RefereeId.Equals(RefereeId)
                    select new GameNomination
                    {
                        Game = g,
                        Nomination  = nom
                    }).ToList<GameNomination>();            
        }
    }
}