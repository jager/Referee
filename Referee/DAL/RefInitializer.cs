using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Referee.Models;

namespace Referee.DAL
{
    public class RefInitializer : CreateDatabaseIfNotExists<RefereeContext>
    {
        protected override void Seed(RefereeContext context)
        {
            var functions = new List<Function>
            {
                new Function { Id = 1001, Name = "I" },
                new Function { Id = 2002, Name = "II" },
                new Function { Id = 3003, Name = "sekretarz"},
                new Function { Id = 4005, Name = "liniowy" },
                new Function { Id = 5007, Name = "główny" },
                new Function { Id = 6011, Name = "I + główny" },
                new Function { Id = 7013, Name = "II + główny" },
                new Function { Id = 8017, Name = "kwalifikator" },
                new Function { Id = 9019, Name = "rezerwowy" }
            };
            functions.ForEach(f => context.Functions.Add(f));
            context.SaveChanges();

            var auth = new List<Authorization>
            {
                new Authorization { Id = 99, Name = "KK" },
                new Authorization { Id = 93, Name = "JM2" },
                new Authorization { Id = 89, Name = "S" }
            };
            auth.ForEach(a => context.Authorizations.Add(a));
            context.SaveChanges();

            var season = new List<Season>
            {
                new Season { Name = "2012/2013 - Sezon Halowy", Active = true, Archive = false, Visible = true },
                new Season { Name = "2012/2013 - Sezon Plażowy", Active = false, Archive = false, Visible = false }
            };

            season.ForEach(s => context.Seasons.Add(s));
            context.SaveChanges();

            var league = new List<League>
            {
                new League { Id = 1, Name = "I Liga młodziczek", Type = "tournament" },
                new League { Id = 11, Name = "I Liga młodzików", Type = "tournament" },

                new League { Id = 10, Name = "I liga kadetek", Type = "league" },
                new League { Id = 20, Name = "II liga kadetek", Type = "league" },
                new League { Id = 30, Name = "I liga kadetów", Type = "league" },
                new League { Id = 40, Name = "II liga kadetów", Type = "league" },
                new League { Id = 50, Name = "I liga juniorek", Type = "league" },
                new League { Id = 60, Name = "II liga juniorek", Type = "league" },
                new League { Id = 70, Name = "I liga juniorów", Type = "league" },
                new League { Id = 80, Name = "II liga juniorów", Type = "league" },
                new League { Id = 90, Name = "III liga seniorów", Type = "league" },
                new League { Id = 100, Name = "III liga seniorek", Type = "league" },
                new League { Id = 110, Name = "IV liga seniorów", Type = "league" },

                new League { Id = 210, Name = "II liga Mężczyzn - Grupa 1", Type = "league" },
                new League { Id = 220, Name = "II liga Mężczyzn - Grupa 2", Type = "league" },
                new League { Id = 230, Name = "II liga Mężczyzn - Grupa 3", Type = "league" },
                new League { Id = 240, Name = "II liga Mężczyzn - Grupa 4", Type = "league" },
                new League { Id = 250, Name = "II liga Kobiet - Grupa 1", Type = "league" },
                new League { Id = 260, Name = "II liga Kobiet - Grupa 2", Type = "league" },
                new League { Id = 270, Name = "II liga Kobiet - Grupa 3", Type = "league" },
                new League { Id = 280, Name = "II liga Kobiet - Grupa 4", Type = "league" },
                new League { Id = 290, Name = "I liga Kobiet", Type = "league" },
                new League { Id = 300, Name = "I liga Mężczyzn", Type = "league" },
                new League { Id = 310, Name = "Plus Liga Kobiet", Type = "league" },
                new League { Id = 320, Name = "Plus Liga Mężczyzn", Type = "league" },
                new League { Id = 400, Name = "Liga Mistrzyń CEV", Type = "league" },
                new League { Id = 410, Name = "Liga Mistrzów CEV", Type = "league" }, 
                new League { Id = 420, Name = "Puchar CEV Kobiet", Type = "league" },
                new League { Id = 430, Name = "Puchar CEV Mężczyzn", Type = "league" },
                new League { Id = 440, Name = "Challenge Cup Kobiet", Type = "league" },
                new League { Id = 450, Name = "Challenge Cup Mężczyzn", Type = "league" },
                
                new League { Id = 600, Name = "Liga akademicka Kobiet", Type = "league" },
                new League { Id = 610, Name = "Liga akademicka Mężczyzn", Type = "league" },
                
                new League { Id = 1000, Name = "Wszystkie ligi" }
            };
            league.ForEach(l => context.Leagues.Add(l));
            context.SaveChanges();

            var refClass = new List<RefClass>
            {
                new RefClass { Name = "Kandydat", Central = false, International = false },
                new RefClass { Name = "III", Central = false, International = false },
                new RefClass { Name = "II", Central = false, International = false },
                new RefClass { Name = "I", Central = false, International = false },
                new RefClass { Name = "I(SC)", Central = true, International = false },
                new RefClass { Name = "Z", Central = true, International = false },
                new RefClass { Name = "P", Central = true, International = false },
                new RefClass { Name = "M", Central = true, International = true }
            };
            refClass.ForEach(r => context.RefClasses.Add(r));
            context.SaveChanges();

            var role = new List<RefereeRole>
            {
                /// Uprawnienia "KK" do sędziowania jako s1 i s2 w Młodzikach, Młodziczkach i Kadetkach
                //new RefereeRole { FunctionId = 1001, AuthorizationId = 99, GameCategoryId = 101 },
                //new RefereeRole { FunctionId = 2002, AuthorizationId = 99, GameCategoryId = 101 },
                //new RefereeRole { FunctionId = 1001, AuthorizationId = 99, GameCategoryId = 102 },
                //new RefereeRole { FunctionId = 2002, AuthorizationId = 99, GameCategoryId = 102 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 99, LeagueId = 10 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 99, LeagueId = 10 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 99, LeagueId = 20 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 99, LeagueId = 20 },

                ///Uprawnienia "JM2" do sędziowania jako s1 i s2 w Młodzikach, Młodziczkach, Kadetkach, Kadetach, Juniorach 2 ligi, Juniorkach
                ///
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 10 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 10 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 20 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 20 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 30 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 30 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 40 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 40 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 50 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 50 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 60 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 60 },
                new RefereeRole { FunctionId = 1001, AuthorizationId = 93, LeagueId = 80 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 93, LeagueId = 80 },

                ///Wszystkie ligi jako s1 i s2
                new RefereeRole { FunctionId = 1001, AuthorizationId = 89, LeagueId = 1000 },
                new RefereeRole { FunctionId = 2002, AuthorizationId = 89, LeagueId = 1000 }
            };
            role.ForEach(r => context.Roles.Add(r));
            context.SaveChanges();

            var clubs = new List<Club>
            {
                new Club { Name = "AS Łomianki", Phone = "606 877 060", Mailadr = "siatkowka@as-lomianki.pl", Zip = "05-092", City = "Łomianki", Address = " Rolnicza 98A", WebSite = "www.as-lomianki.pl" },
                new Club { Name = "ASPS Pekpol Ostrołęka", Phone = "29 764 44 63 29 764 68 32", Mailadr = "", Zip = "07-400", City = "Ostrołęka", Address = " 11 listopada 20", WebSite = "www.pekpol.net" },
                new Club { Name = "AZS AWF Warszawa", Phone = "22 834 37 94", Mailadr = "sekretariat@azs-awf.waw.pl", Zip = "01-813", City = "Warszawa", Address = "Marymoncka 34", WebSite = "www.azs-awf.waw.pl" },
                new Club { Name = "AZS Politechnika Warszawska", Phone = "22 234 53 71", Mailadr = "siatkowka@azspw.pl", Zip = "00-631", City = "Warszawa", Address = "Waryńskiego 12a", WebSite = "www.azspw.com" },
                new Club { Name = "AZS Uniwersytet Warszawski", Phone = "22 552 04 53", Mailadr = "azs@uw.edu.pl", Zip = "00-325 ", City = "Warszawa", Address = "Krakowskie Przedmieście 24", WebSite = "www.azs.uw.edu.pl" },
                new Club { Name = "GTS Wilga Garwolin", Phone = "25 682 33 71", Mailadr = "splgarwolin@poczta.onet.pl", Zip = "08-400", City = "Garwolin", Address = "Sportowa 34", WebSite = "www.gtswilga.garwolin.org" },
                new Club { Name = "GLKS Nadarzyn", Phone = "22 739 90 18", Mailadr = "klub@glksnadarzyn.pl", Zip = "05-830", City = "Nadarzyn", Address = "Żółwińska 20", WebSite = "www.siatkowka.glksnadarzyn.pl" },
                new Club { Name = "GULKS Zorza Sterdyń", Phone = "25 787 01 42", Mailadr = "", Zip = "08-320", City = "Sterdyń", Address = "Wojska Polskiego 6", WebSite = "" },
                new Club { Name = "KKS Armat Kozienice", Phone = "609 024 660", Mailadr = "mzks.kozienice@mwzps.pl", Zip = "26-900", City = "Kozienice", Address = "Bohaterów Studzianek 30", WebSite = "" },
                new Club { Name = "KPS Jadar Siedlce", Phone = "508 773 524", Mailadr = "kps.siedlce@gmail.com", Zip = "08-110", City = "Siedlce", Address = "Niepodległości 10A", WebSite = "www.kps.siedlce.pl" },
                new Club { Name = "KPS Wołomin", Phone = "600 235 178", Mailadr = "", Zip = "05-200", City = "Wołomin", Address = "Korsaka 4", WebSite = "" },
                new Club { Name = "KS AZS Politechnika Radomska", Phone = "48 361 79 62", Mailadr = "azs@pr.radom.pl", Zip = "26-600", City = "Radom", Address = "Chrobrego 27", WebSite = "www.azs.pr.radom.pl" },
                new Club { Name = "KS Camper Wyszków", Phone = "", Mailadr = "kscamper@home.pl", Zip = "07-200", City = "Wyszków", Address = "Pułtuska 177", WebSite = "www.kscamper.pl" },
                new Club { Name = "KS Metro Warszawa", Phone = "501 027 746", Mailadr = "wojtek@volley.pl", Zip = "02-776", City = "Warszawa", Address = "Hirszfelda 11", WebSite = "www.ksmetro.pl" },
                new Club { Name = "KS Mogielanka Mogielnica", Phone = "48 663 51 49", Mailadr = "ksmogielanka@op.pl", Zip = "05-640", City = "Mogielnica", Address = "Rynek 15 lok. 2", WebSite = "www.mogielnica.pl" },
                new Club { Name = "KS Nadstal Krzaki Czaplinkowskie", Phone = "22 727 39 19", Mailadr = "nadstal@nadstal.waw.pl", Zip = "05-530", City = "Góra Kalwaria", Address = "Krzaki Czaplinkowskie 40", WebSite = "www.ksnadstal.waw.pl" },
                new Club { Name = "KS Sparta Nasielsk", Phone = "", Mailadr = "", Zip = "05-800", City = "Nasielsk", Address = " Staszica 1", WebSite = "" },
                new Club { Name = "KS Zawkrze Mława", Phone = "23 652 00 29", Mailadr = "sekretariat@mhsmlawa.pl", Zip = "06-500", City = "Mława", Address = "Piłsudskiego 33A", WebSite = "www.mhsmlawa.pl" },
                new Club { Name = "KS Żyrardowianka Żyrardów", Phone = "", Mailadr = "zyrardowianka@zyrardowianka.pl", Zip = "96-300", City = "Żyrardów", Address = "Jodłowskiego 25/27", WebSite = "www.zyrardowianka.home.pl" },
                new Club { Name = "LKS Wrzos Międzyborów", Phone = "", Mailadr = "", Zip = "96-316", City = " Międzyborów", Address = "Staszica 5", WebSite = "" },
                new Club { Name = "LTS Legionovia Legionowo", Phone = "22 784 41 45", Mailadr = "biuro@lts.legionovia.pl.pl", Zip = "05-120", City = "Legionowo", Address = "Parkowa 27A", WebSite = "www.lts.legionovia.info" },
                new Club { Name = "LUKS Set Lesznowola", Phone = "22 757 93 99", Mailadr = "set.lesznowola@mwzps.pl ", Zip = "05-506", City = "Lesznowola", Address = "Szkolna 6", WebSite = "" },
                new Club { Name = "MKS MDK Warszawa", Phone = "660 635 005", Mailadr = "azelazny@akacjowa.pl", Zip = "00-449", City = "Warszawa", Address = "Łazienkowska 7", WebSite = "www.mdkwawa.prv.pl" },
                new Club { Name = "MKS MOS Pruszków", Phone = "22 758 22 91", Mailadr = "mospruszkow@o2.pl", Zip = "05-800", City = "Pruszków", Address = "Gomulińskiego 4", WebSite = "" },
                new Club { Name = "MKS Olimp Mińsk Mazowiecki", Phone = "25 758 51 02", Mailadr = "", Zip = "05-300", City = "Mińsk Maz", Address = "Piękna 7A", WebSite = "" },
                new Club { Name = "MMKS Mińsk Mazowiecki", Phone = "", Mailadr = "", Zip = "05-300", City = "Mińsk Maz", Address = "Szczecińska 16 G", WebSite = "" },
                new Club { Name = "MTS Marcovia 2000 Marki", Phone = "22 781 12 84", Mailadr = "", Zip = "05-260", City = "Marki", Address = "Wspólna 12", WebSite = "" },
                new Club { Name = "MUKS Sparta Warszawa", Phone = "504 177 177", Mailadr = "sparta.wwa@mwzps.pl", Zip = "03-580", City = "Warszawa", Address = "Askenazego 9/19", WebSite = "www.spartawarszawa.pl" },
                new Club { Name = "MUKS Volley Płock", Phone = "24 364 06 40", Mailadr = "", Zip = "09-409", City = "Płock", Address = "Łukasiewicza 11", WebSite = "" },
                new Club { Name = "MUKS Volley Wołomin", Phone = "22 776 34 36", Mailadr = "muks_volley@o2.pl", Zip = "05-200", City = "Wołomin", Address = "Sasina 33", WebSite = "www.volleywolomin.pl" },
                new Club { Name = "NOSiR Nowy Dwór Mazowiecki", Phone = "22 775 41 42", Mailadr = "nosir@nowydwormaz.pl", Zip = "05-100", City = "Nowy Dwór Maz", Address = "Sportowa 66", WebSite = "www.nosir.nowydwormaz.pl" },
                new Club { Name = "OSiR Żyrardów", Phone = "46 855 42 35", Mailadr = "osir@zyrardow.pl", Zip = "96-300", City = "Żyrardów", Address = "Okrzei 59", WebSite = "www.osir.zyrardow.pl" },
                new Club { Name = "OTPS Nike Ostrołęka", Phone = "", Mailadr = "nike-ostroleka@o2.pl", Zip = "07-410", City = "Ostrołęka", Address = "Wincentego Pola 5", WebSite = "www.nike-ostroleka.pl" },
                new Club { Name = "PKS Bogurzyn", Phone = "23 655 80 11", Mailadr = "", Zip = "06-521", City = "Wiśniewo", Address = "Bogurzyn 26", WebSite = "" },
                new Club { Name = "PTS Pionki", Phone = "48 612 96 22", Mailadr = "pts.pionki@mwzps.pl", Zip = "26-670", City = "Pionki", Address = "Złota 3", WebSite = "" },
                new Club { Name = "PTS Płońsk", Phone = "23 662 27 01", Mailadr = "jaroslaw.jeznach@wp.pl", Zip = "09-100 ", City = "Płońsk", Address = "Kopernika 3", WebSite = "www.mcsir.plonsk.pl" },
                new Club { Name = "RCS Czarni Radom", Phone = "48 385 10 01 w. 37", Mailadr = "radcentrsiat@interia.pl", Zip = "26-600", City = "Radom", Address = "Narutowicza 9", WebSite = "www.czarni.radom.pl" },
                new Club { Name = "SKF Global Village Mokobody", Phone = "502 290 325", Mailadr = "a_kamin1@wp.pl", Zip = "08-124", City = "Mokobody", Address = "Ossolińskich 21", WebSite = "www.globalvillage.cba.pl" },
                new Club { Name = "SKF Radomka Radom", Phone = "48 366 41 30", Mailadr = "radomka@mwzps.pl", Zip = "26-600", City = "Radom", Address = "Tartaczna 5", WebSite = "www.radomka.com.pl" },
                new Club { Name = "SPS Olimpia Węgrów", Phone = "", Mailadr = "siatka543@gmail.com", Zip = "07-100", City = "Węgrów", Address = "Kościuszki 16", WebSite = "www.olimpia-wegrow.dzs.pl" },
                new Club { Name = "UKS 33 Radom", Phone = "48 363 73 62", Mailadr = "danuta.fryszkowska@neostrada.pl", Zip = "26-600", City = "Radom", Address = "Kolberga 5", WebSite = "" },
                new Club { Name = "UKS Amur Wilga", Phone = "", Mailadr = "", Zip = "", City = "", Address = "", WebSite = "" },
                new Club { Name = "UKS Atena Warszawa", Phone = "606 650 979", Mailadr = "uksatena@tlen.pl", Zip = "02-693", City = "Warszawa", Address = "Gruszczyńskiego 12", WebSite = "www.uksatena.waw.pl" },
                new Club { Name = "UKS Beta Błonie", Phone = "665 392 057", Mailadr = "uks.beta.blonie@gmail.com", Zip = "05-870", City = "Błonie", Address = "Narutowicza 4", WebSite = "www.uksblonie.republika.pl" },
                new Club { Name = "UKS Beta Pionki", Phone = "48 612 55 95", Mailadr = "uksbetapionki@poczta.onet.pl", Zip = "26-670", City = "Pionki", Address = "Słowackiego 4", WebSite = "www.uksbeta.pionki.com.pl" },
                new Club { Name = "UKS Cirkus Konstancin Jeziorna", Phone = "22 754 03 57", Mailadr = "jacek.nowicki3@wp.pl", Zip = "05-520", City = " Konstancin Jeziorna", Address = "Mirkowska 39", WebSite = "" },
                new Club { Name = "UKS Dębina Nieporęt", Phone = "22 772 30 30", Mailadr = "info@uksdebina.pl", Zip = "05-126", City = "Nieporęt", Address = "Jana Kazimierza 16 D", WebSite = "www.uksdebina.pl" },
                new Club { Name = "UKS Dwójka Maków Mazowiecki", Phone = "29 717 15 82", Mailadr = "", Zip = "06-200", City = "Maków Maz", Address = "Pułaskiego 15", WebSite = "" },
                new Club { Name = "UKS Gocław 75 Warszawa", Phone = "728 330 465", Mailadr = "volleyball@uksgoclaw75.pl", Zip = "03-982", City = "Warszawa", Address = " Bartosika 5", WebSite = "www.uksgoclaw75.pl" },
                new Club { Name = "UKS Halinów", Phone = "502 600 543", Mailadr = "", Zip = "05-074", City = "Halinów", Address = "Okuniewska 115", WebSite = "" },
                new Club { Name = "UKS Iskra - Volley Zielona", Phone = "23 257 90 51", Mailadr = "zpozielona@op.pl", Zip = "09-310", City = " Kuczbork  Zielona", Address = "Szkolna 2", WebSite = "www.iskra.jpg.pl" },
                new Club { Name = "UKS Jargoś Kobyłka", Phone = "22 786 15 71", Mailadr = "", Zip = "05-230", City = "Kobyłka", Address = "Załuskiego 5", WebSite = "" },
                new Club { Name = "UKS Jedynka Kozienice", Phone = "601 054 674", Mailadr = "jedynka.kozienice@mwzps.pl", Zip = "26-900", City = "Kozienice", Address = "Kościuszki 1", WebSite = "" },
                new Club { Name = "UKS Jedynka Nowy Dwór Mazowiecki", Phone = "22 775 91 23", Mailadr = "", Zip = "05-100", City = "Nowy Dwór Maz", Address = "Słowackiego 2", WebSite = "" },
                new Club { Name = "UKS Jedynka Siedlce", Phone = "25 632 33 65", Mailadr = "gim1siedlce@wp.pl", Zip = "08-110", City = "Siedlce", Address = "Konarskiego 5/7", WebSite = "" },
                new Club { Name = "UKS Laura Chylice", Phone = "", Mailadr = "kjtrener@wp.pl", Zip = "05-510", City = "Konstancin Jeziorna", Address = "Dworska 5", WebSite = "" },
                new Club { Name = "UKS Mega-Gim Zawidz Kościelny", Phone = "24 276 61 97", Mailadr = "gimzaw@plock.edu.pl", Zip = "09-226", City = "Zawidz Kościelny", Address = "Mazowiecka 47B", WebSite = "" },
                new Club { Name = "UKS Mszczonowianka Mszczonów", Phone = "46 857 27 47", Mailadr = "osir@mszczonow.pl", Zip = "96-320", City = "Mszczonów", Address = "Tarczyńska 31", WebSite = "" },
                new Club { Name = "UKS Olimp Ostrołęka", Phone = "29 764 45 40", Mailadr = "", Zip = "07-412", City = "Ostrołęka", Address = "Hallera 12", WebSite = "" },
                new Club { Name = "UKS Olimp Tłuszcz", Phone = "29 757 30 02", Mailadr = "uksolimptluszcz@republika.onet.pl", Zip = "05-240", City = "Tłuszcz", Address = " Kościelna 1", WebSite = "www.uksolimptluszcz.republika.pl" },
                new Club { Name = "UKS Opia 2008 Opinogóra", Phone = "23 671 70 16", Mailadr = "", Zip = "06-406", City = "Opinogóra Grn", Address = "Krasińskiego 25", WebSite = "" },
                new Club { Name = "UKS Orzeł Unin", Phone = "25 683 03 48", Mailadr = "", Zip = "08-400", City = "Garwolin", Address = "Unin 68", WebSite = "" },
                new Club { Name = "UKS Ósemka Pruszków", Phone = "695 589 361", Mailadr = "osemka.pruszkow@mwzps.pl", Zip = "05-804", City = "Pruszków", Address = "Wokulskiego 11/3", WebSite = "www.uks-osemka.com" },
                new Club { Name = "UKS Ósemka Siedlce", Phone = "25 632 97 10", Mailadr = "osemkasiedlce@home.pl", Zip = "08-101", City = "Siedlce", Address = "ul. Żuławska 1", WebSite = "www.osemkasiedlce.pl" },
                new Club { Name = "UKS Piaski Legionowo", Phone = "22 774 59 50", Mailadr = "", Zip = "05-119", City = "Legionowo", Address = "Zegrzyńska 3", WebSite = "" },
                new Club { Name = "UKS Plas Warszawa", Phone = "509 692 150", Mailadr = "", Zip = "03-329", City = "Warszawa", Address = " Balkonowa 2/4", WebSite = "www.plas.waw.pl" },
                new Club { Name = "UKS Polonez Wyszków", Phone = "29 742 45 59", Mailadr = "", Zip = "07-200", City = "Wyszków", Address = "Geodetów 45", WebSite = "" },
                new Club { Name = "UKS Powsinek Wilanów Warszawa", Phone = "22 842 99 89", Mailadr = "", Zip = "02-967", City = "Warszawa", Address = "Uprawna 9/17", WebSite = "" },
                new Club { Name = "UKS Pułaski Warszawa", Phone = "22 619 11 20 w. 15", Mailadr = "ukspulaski@gmail.com", Zip = "03-481 ", City = "Warszawa", Address = "Szanajcy 17/19", WebSite = "www.ukspulaski.yoyo.pl" },
                new Club { Name = "UKS Salos Legionowo", Phone = "502 532 429", Mailadr = "olo22.09.91@interia.pl", Zip = "05-120", City = "Legionowo", Address = "Mickiewicza 35A", WebSite = "www.salos.eu" },
                new Club { Name = "UKS Sparta Grodzisk Mazowiecki", Phone = "22 755 55 26", Mailadr = "ukssparta@wp.pl", Zip = "05-825", City = "Grodzisk Maz", Address = "Zondka 6", WebSite = "www.sparta.csd.pl" },
                new Club { Name = "UKS Start Lipowiec Kościelny", Phone = "23 655 50 50", Mailadr = "siatkowka.lipowiec@vp.pl", Zip = "06-545", City = "Lipowiec Kościelny", Address = "Lipowiec 212", WebSite = "www.siatkowkalipowiec.republika.pl" },
                new Club { Name = "UKS Victoria Józefów", Phone = "22 789 11 77", Mailadr = "piotrgluszyk@op.pl", Zip = "05-420", City = "Józefów", Address = "Długa 44", WebSite = "www.victoriajozefow.pl" },
                new Club { Name = "UKS Zryw Płutusk", Phone = "23 692 06 41", Mailadr = "kontakt@szs.pultusk.edu.pl", Zip = "06-100", City = " Pułtusk", Address = "Tysiąclecia 12", WebSite = "www.pultusksiatka.friko.pl" },
                new Club { Name = "ULKS Wilga Miastków", Phone = "25 751 09 76", Mailadr = "pg.miastkow@poczta.onet.pl", Zip = "08-420", City = "Miastków Kościelny", Address = "Szkolna 8", WebSite = "" },
                new Club { Name = "UMKS MOS Wola Warszawa", Phone = "22 631 49 89", Mailadr = "klub@moswola.pl", Zip = "01-206", City = "Warszawa", Address = "Rogalińska 2", WebSite = "www.moswola.pl" },
                new Club { Name = "UMKS Norwid Wyszków", Phone = "29 742 46 22", Mailadr = "", Zip = "07-200", City = "Wyszków", Address = "11 listopada 1", WebSite = "www.umks-norwid.prv.pl" },
                new Club { Name = "WUKS Junior Stolarka Wołomin", Phone = "22 787 59 26", Mailadr = "szerszeniewska@wp.pl", Zip = "05-200", City = "Wołomin", Address = "Lipińska 16", WebSite = "" }
            };
            clubs.ForEach(c => context.Clubs.Add(c));
            context.SaveChanges();
        }
    }
}