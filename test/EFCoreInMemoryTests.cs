using System;
using System.Drawing;
using System.Linq;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace test {
    public class EFCoreInMemoryTests {

        private static Team CreateTeamAjax () {
            return new Team ("AFC Ajax", "The Lancers", "1900", "Amsterdam Arena");
        }

       
[Fact]
        public void CanStoreAndRetrieveHomeColors () {
            var team = CreateTeamAjax ();
            team.SpecifyHomeUniformColors (Color.White, Color.Red, Color.Empty, Color.White, Color.Empty, Color.White);
             var options = new DbContextOptionsBuilder<TeamContext> ().UseInMemoryDatabase ("onandonly").Options;
         
        using (var context = new TeamContext (options)) {
                context.Teams.Add (team);
                context.SaveChanges ();
            }
            using (var context = new TeamContext(options))
            {
                var storedTeam = context.Teams.Include(t => t.HomeColors).FirstOrDefault();

                Assert.Equal(Color.Blue, storedTeam.HomeColors.ShirtPrimary);
            }
        }
    }
}