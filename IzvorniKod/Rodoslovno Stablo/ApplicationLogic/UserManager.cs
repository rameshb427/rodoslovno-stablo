﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;

using ServiceStack.Common.Utils;
using ServiceStack.DataAnnotations;
using ServiceStack.Common.Extensions;
using System.Reflection;

using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;


namespace ApplicationLogic
{
	public class User
	{	// TODO ostatak
		public User() { }
		[AutoIncrement]
		[Alias("UserID")]
		public Int32 ID { get; set; }

		[Index(Unique=true)]
		[StringLength(10)]
		public string username { get; set; }

		[StringLength(10)]
		public string password { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public bool isAdmin { get; set; }

        public Int32 adminID { get; set; } // veza na User

        // veza sa stablom, totalno nepotrebna duplikacija podataka
        public Guid PersonID { get; set; } // veza na Person
        public string name { get; set; }
        public string surname { get; set; }
        public DateTime birthDate { get; set; }
        public DateTime deathDate { get; set; }
        public Person.Sex sex;
        public Image photo { get; set; } // TODO
        public string address { get; set; }
        public string CV { get; set; }

        public Guid PartnerID { get; set; } // veza na Person
        public string partnerName { get; set; }
        public string partnerSurname { get; set; }
        public Guid MarriageID { get; set; } // veza na Connection
        public DateTime marriedDate { get; set; }
	}

    public class Query
    {
        [AutoIncrement]
        [Alias("QueryID")]
        public Int32 ID { get; set; }
        [References(typeof(User))]
        public Int32 UserID { get; set; } // veza na User, korisnik koji je postavio upit
        public string command { get; set; }
    }


	public class UserManager
	{
		// Izvor za orm  https://github.com/ServiceStack/ServiceStack.OrmLite
		// inspiracija za ovaj kod https://github.com/ServiceStack/ServiceStack.OrmLite/blob/master/src/SqliteExpressionsTest/Program.cs

		// rijesenje buga sa loadanjem .dll-a http://mdetras.com/2011/09/29/unable-to-load-dll-sqlite-interop-dll/

		private static string GetFileConnectionString()
		{
			var connectionString = "~/db.sqlite".MapAbsolutePath();
			if (File.Exists(connectionString))
				File.Delete(connectionString); // TODO da ipak ne pobrise bazu

			System.Console.WriteLine("Baza na lokaciji {0}", connectionString);
			return connectionString;
		}

        private string connectionString;

        private void DBInit()
        {
            OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;

            connectionString = GetFileConnectionString();
            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                // users
                db.DropTable<User>();
                db.CreateTable<User>();
                db.DeleteAll<User>();

                List<User> users = new List<User>();
                users.Add(new User() { username = "admin", password = "abc123" });

                db.InsertAll(users);

                // queries
                db.DropTable<Query>();
                db.CreateTable<Query>();
                db.DeleteAll<Query>();

                List <Query> queries = new List<Query>();
                queries.Add(new Query()
                {
                    UserID = db.Select<User>(x => x.username == "admin").First().ID,
                    command = "ispisi_stablo"
                });
                queries.Add(new Query()
                {
                    UserID = db.Select<User>(x => x.username == "admin").First().ID,
                    command = "ispisi_osobu Chuck, Norris"
                });
                queries.Add(new Query()
                {
                    UserID = db.Select<User>(x => x.username == "admin").First().ID,
                    command = "dodaj_osobu Chuck, Norris"
                });

                db.InsertAll(queries);
            }
        }
		public UserManager()
		{
            DBInit();
		}

        private User ActiveUser;

        public void AddUser(User korisnik)
        {
            List<User> korisnici = null;

            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                korisnici = db.Select<User>(x => x.username == korisnik.username);

                if (korisnici.Count() != 0)
                {
                    throw new System.ArgumentException("Username je vec zauzet");
                }

                db.Insert(korisnik);
            }
        }

        public User GetActiveUser()
        { 
            return ActiveUser;
        }

        public void UpdateUser(User korisnik)
        {
            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                db.Update<User>(korisnik);
            }
        }

        public void StoreQuery(Query upit)
        {
            upit.UserID = ActiveUser.ID;
            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                db.Insert<Query>(upit);
            }
        }

        public IEnumerable<Query> GetQueries()
        {   
            IEnumerable<Query> ret = null;

            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                ret = db.Select<Query>(x => x.UserID == ActiveUser.ID);
            }

            return ret;
        }

		public bool Login(string username, string password)
		{
			// TODO, dodat password hashing

            List<User> aktivni = null;
            using (IDbConnection db = connectionString.OpenDbConnection())
            {
                aktivni = db.Select<User>(x => x.username == username && x.password == password);
            }

            if (aktivni.Count != 1)
            {
                return false;
            }

            ActiveUser = aktivni.ElementAt(0);
			return true;
		}

		public bool Logoff()
		{
            ActiveUser = null;
			return true;
		}
	}
}
