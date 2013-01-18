﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ApplicationLogic
{
	public partial class QueryProcessor
	{
		//private Tree Drvo;
		// za potrebe testiranja
		public Tree Drvo;

		// Podesiva funkcija za odlucivanje o kojoj osobi se radi kada je upit dvosmislen
		private Func<IEnumerable<Person>, string, Person> QueryDisambiguator;

		// Podesiva funkcija za dohvacanje korisnikovog unosa
		// ovo je jedina funkcija koju mozemo koristiti za to
		private Func<string> GetLine;

		public QueryProcessor(Func<IEnumerable<Person>, string, Person> QD, 
								Func<string> daj_liniju, TextWriter tw = null)
		{
			Drvo = new Tree();

			// dodajmo malo couplinga
			QueryDisambiguator = QD;
			GetLine = daj_liniju;

			// preusmjeri System.Console.Out tamo gdje nam odgovara da mozemo koristiti System.Console.WriteLine
			if (tw != null)		// Da omogucimo ovo http://saezndaree.wordpress.com/2009/03/29/how-to-redirect-the-consoles-output-to-a-textbox-in-c/
				System.Console.SetOut(tw);

			InitializeCommands();
		}

		// tablica sa mapiranjem komandi i kljucnih rijeci, zajedno sa opisima
		private List<CommandDescriptor> komande;

		private class CommandDescriptor
		{
			public List<string> keywords { get; set; }
			public string description { get; set; }
			public Action<string[]> func { get; set;} 

			public CommandDescriptor(string[] words, Action<string[]> funk, string desc = null)
			{
				Initialize(words, funk, desc);
			}

			public CommandDescriptor(string word, Action<string[]> funk, string desc = null) 
			{
				Initialize(new string[] { word }, funk, desc);
			}
			
			public void Initialize(string[] words, Action<string[]> funk, string desc)
			{ 
				keywords = new List<string>();
				keywords.AddRange(words);

				func = funk;
				description = desc;
			}
		}

		public void InitializeCommands()
		{
			komande = new List<CommandDescriptor>();
			komande.Add(new CommandDescriptor("dodaj_osobu", AddPerson, "dodaj_osobu ime, prezime"));
			komande.Add(new CommandDescriptor("nadji_osobu", GetPerson, "nadji_osobu ime, prezime"));
			komande.Add(new CommandDescriptor("dodaj_baku", AddGrandmother, "dodaj_baku ime_unuk, prezime_unuk, ime, prezime"));
			komande.Add(new CommandDescriptor("dodaj_djeda", AddGrandfather, "dodaj_djeda ime_unuk, prezime_unuk, ime, prezime"));
			komande.Add(new CommandDescriptor("dodaj_praroditelja", AddGrandparent, "dodaj_praroditelja ime_unuk, prezime_unuk, ime, prezime"));
            komande.Add(new CommandDescriptor("dohvati_sve_rodjene_izmedju", Dohvati_sve_rodjene_izmedju, "dohvati_sve_rodjene_izmedju datum1, datum2"));
            komande.Add(new CommandDescriptor("dohvati_sve_umrle_izmedju", Dohvati_sve_umrle_izmedju, "dohvati_sve_umrle_izmedju datum1, datum2"));
            komande.Add(new CommandDescriptor("dohvati_sve_koji_pozivjese_vise_od", Dohvati_sve_koji_pozivjese_vise_od, "dohvati_sve_koji_pozivjese_vise_od brojGodina"));
            komande.Add(new CommandDescriptor("dohvati_sve_koji_pozivjese_manje_od", Dohvati_sve_koji_pozivjese_manje_od, "dohvati_sve_koji_pozivjese_manje_od brojGodina"));
            komande.Add(new CommandDescriptor("razlika_u_starosti", Razlika_u_starosti, "razlika_u_starosti ime1, prezime1, ime2, prezime2"));
			komande.Add(new CommandDescriptor("nadji_bake", GetGrandmother, "nadji_bake ime_unuk, prezime_unuk"));
			komande.Add(new CommandDescriptor("nadji_djedove", GetGrandfather, "nadji_djedove ime_unuk, prezime_unuk"));
			komande.Add(new CommandDescriptor("nadji_praroditelje", GetGrandparent, "nadji_praroditelje ime_unuk, prezime_unuk"));
			komande.Add(new CommandDescriptor("promijeni_podatke", ChangeData, "promijeni_podatke ime, prezime"));
			komande.Add(new CommandDescriptor("ispisi_stablo", PrintTree, "ispisi_stablo"));
			komande.Add(new CommandDescriptor("ispisi_osobu", PrintPerson, "ispisi_osobu ime, prezime"));
			komande.Add(new CommandDescriptor("izlaz", Quit, "izlaz"));            
			// TODO popis funkcija
		}
	}
}
