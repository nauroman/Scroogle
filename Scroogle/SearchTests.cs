using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace TestApplication
{
	static class SearchTests
	{
		static public void Tests()
		{
			DateTime end;
			DateTime start = DateTime.Now;

			Console.WriteLine("### Overall Start Time: " + start.ToLongTimeString());
			Console.WriteLine();

			#region TestFastestStructureForStringLookup
			//Fastest string lookup
			//TestFastestStructureForStringLookup(1, 12);
			TestFastestStructureForStringLookup(100, 12);
			TestFastestStructureForStringLookup(100, 50);
			TestFastestStructureForStringLookup(100, 128);
			TestFastestStructureForStringLookup(5000, 12);
			TestFastestStructureForStringLookup(5000, 50);
			TestFastestStructureForStringLookup(5000, 128);
			TestFastestStructureForStringLookup(25000, 12);
			TestFastestStructureForStringLookup(25000, 50);
			TestFastestStructureForStringLookup(25000, 128);
			TestFastestStructureForStringLookup(100000, 12);
			TestFastestStructureForStringLookup(100000, 50);
			TestFastestStructureForStringLookup(100000, 128);
			#endregion

			end = DateTime.Now;
			Console.WriteLine();
			Console.WriteLine("### Overall End Time: " + end.ToLongTimeString());
			Console.WriteLine("### Overall Run Time: " + (end - start));

			Console.WriteLine();
			Console.WriteLine("Hit Enter to Exit");
			Console.ReadLine();

		}

		//###############################################################

		static string GetRandomString(int LengthOfStrings)
		{
			var chars = "abcdefghijklmnopqrstuvwxyz";
			var stringChars = new char[LengthOfStrings];
			var random = new Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			var finalString = new String(stringChars);
//			ss[x] = System.Web.Security.Membership.GeneratePassword(LengthOfStrings, x % 5);


			return finalString;
		}

		//what is the fastest structure to lookup a string?
		//hashset.contains
		//dictionary key
		//concurrent dictionary key
		//sorted dictionary
		//list.containsvalue
		//array
		static void TestFastestStructureForStringLookup(int NumberOfStringsToGenerate, int LengthOfStrings)
		{
			Console.WriteLine("######## " + System.Reflection.MethodBase.GetCurrentMethod().Name);
			Console.WriteLine("Number of Random Strings that will be generated: " + NumberOfStringsToGenerate.ToString("#,##0"));
			Console.WriteLine("Length of Random Strings that will be generated: " + LengthOfStrings.ToString("#,##0"));
			Console.WriteLine();

			object lockObject = new object();
			int total = 0;
			DateTime end = DateTime.Now;
			DateTime start = DateTime.Now;
			string temp_str = String.Empty;

			string[] a = new string[NumberOfStringsToGenerate];
			string[] a_bs = new string[NumberOfStringsToGenerate];  //for a binary search
			var l = new List<string>(NumberOfStringsToGenerate);
			var l_bs = new List<string>(NumberOfStringsToGenerate); //for binary search
			SortedList sl = new SortedList(NumberOfStringsToGenerate);
			ArrayList al = new ArrayList(NumberOfStringsToGenerate);
			Dictionary<string, string> d = new Dictionary<string, string>(NumberOfStringsToGenerate);
			ConcurrentDictionary<string, string> cd = new ConcurrentDictionary<string, string>(Environment.ProcessorCount, NumberOfStringsToGenerate);
			SortedDictionary<string, string> sd = new SortedDictionary<string, string>();
			var hs = new HashSet<string>();
			Hashtable ht = new Hashtable(NumberOfStringsToGenerate);

			//the strings to look up
			string[] ss = new string[NumberOfStringsToGenerate];

			//Generate the string arrays that all the structures will read from.
			//This is to make sure every structure uses the same set of strings during the run.
			//strings to be searched. Completely random. Using generate password method to come up with all sorts of mixtures.
			Console.WriteLine("Generating strings to look up.");
			for (int x = 0; x < NumberOfStringsToGenerate; x++)
			{
				//				ss[x] = System.Web.Security.Membership.GeneratePassword(LengthOfStrings, x % 5);

				ss[x] = GetRandomString(LengthOfStrings);

				if (x % 3 == 0)
					temp_str = ss[x]; //to ensure there's always at least a few matches
				else
					temp_str = GetRandomString(LengthOfStrings);
				//temp_str = System.Web.Security.Membership.GeneratePassword(LengthOfStrings, x % 5);

				//so all the collections have the exact same strings.
				a[x] = temp_str;
				a_bs[x] = temp_str;
				al.Add(temp_str);
				l.Add(temp_str);
				l_bs.Add(temp_str);
				sl.Add(temp_str, temp_str);
				if (!d.ContainsKey(temp_str))
				{
					d.Add(temp_str, temp_str);
					sd.Add(temp_str, temp_str);
					cd[temp_str] = temp_str;
				}
				hs.Add(temp_str);
				ht.Add(temp_str, temp_str);
			}
			Array.Sort(a_bs);   //presort the binarysearch array as otherwise the results may be incorrect.
			l_bs.Sort();    //presort 

			Console.WriteLine("###########################################################");
			Console.WriteLine();

			total = 0;
			Console.WriteLine("Starting searching an array... " + start.ToLongTimeString());
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				for (int y = 0; y < a.Length; y++)
				{
					if (a[y].Contains(ss[x]))
					{   //found it.
						total += 1;
						break;
					}
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching an array using the custom method... " + start.ToLongTimeString());
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				for (int y = 0; y < a.Length; y++)
				{
					if ((ss[x].Length - ss[x].Replace(a[y], String.Empty).Length) / a[y].Length > 0)
					{   //found it.
						total += 1;
						break;
					}
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting binary searching an array... " + start.ToLongTimeString());
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				int y = Array.BinarySearch(a_bs, ss[x]); if (y >= 0)
					total += 1;
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching an ArrayList using contains... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (al.Contains(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching an ArrayList using a for loop... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				for (int y = 0; y < al.Count; y++)
				{
					if (al[y] == ss[x])
					{   //found it.
						total += 1;
						break;
					}
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a List using Contains... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (l.Contains(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a List using a for loop... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				for (int y = 0; y < l.Count; y++)
				{
					if (l[y] == ss[x])
					{   //found it.
						total += 1;
						break;
					}
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting binary searching a List... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				int y = l_bs.BinarySearch(ss[x]); if (y >= 0)
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a SortedList... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (sl.ContainsKey(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a Dictionary Key... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (d.ContainsKey(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a Dictionary Value... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (d.ContainsValue(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a SortedDictionary Key... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (sd.ContainsKey(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a SortedDictionary Value... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (sd.ContainsValue(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a ConcurrentDictionary Key... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (cd.ContainsKey(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a ConcurrentDictionary Value... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (cd.Values.Any(z => z == ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a HashSet... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (hs.Contains(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a HashTable Key... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (ht.ContainsKey(ss[x]))
				{   //found it.
					total += 1;
				}
			}
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a HashTable Value... ");
			start = DateTime.Now;
			for (int x = 0; x < ss.Length; x++)
			{
				if (ht.ContainsValue(ss[x]))
				{   //found it.              
					total += 1;                 }             }             end = DateTime.Now;             Console.WriteLine("Finished at: " + end.ToLongTimeString());             Console.WriteLine("Time: " + (end - start));             Console.WriteLine("Total finds: " + total + Environment.NewLine);             Console.WriteLine();             Console.WriteLine("###########################################################");             Console.WriteLine();             Console.WriteLine("###########################################################");             Console.WriteLine("#########Starting the parallel implementations#############");             Console.WriteLine("############################################################");             total = 0;             Console.WriteLine("Starting parallel searching an array... " + start.ToLongTimeString());             start = DateTime.Now;             Parallel.For(0, ss.Length,                 () => 0,
					(x, loopState, subtotal) =>
					{
						for (int y = 0; y < a.Length; y++)
						{
							if (a[y].Contains(ss[x]))
							{   //found it.          
								subtotal += 1;                             break;                         }                     }                     return subtotal;                 },                 (s) =>
								{
									lock (lockObject)
									{
										total += s;
									}
								}
            );
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching an array using custom method... " + start.ToLongTimeString());
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					for (int y = 0; y < a.Length; y++)
					{
						if ((ss[x].Length - ss[x].Replace(a[y], String.Empty).Length) / a[y].Length > 0)
						{   //found it.
							subtotal += 1;
							break;
						}
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel binary searching an array... " + start.ToLongTimeString());
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					int y = Array.BinarySearch(a_bs, ss[x]);
					if (y >= 0)
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching an ArrayList using contains... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (al.Contains(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching an ArrayList using a for loop... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
				for (int y = 0; y < al.Count; y++)
				{
					if (al[y] == ss[x])
					{   //found it.            
						subtotal += 1;                             break;                         }                     }                     return subtotal;                 },                 (s) =>
						{
							lock (lockObject)
							{
								total += s;
							}
						}

			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a List using contains... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (l.Contains(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a List using a for loop... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					for (int y = 0; y < l.Count; y++)
					{
						if (l[y] == ss[x])
						{   //found it.                  
						total += 1;                             break;                         }                     }                     return subtotal;                 },                 (s) =>
							{
								lock (lockObject)
								{
									total += s;
								}
							}

			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel binary searching a List... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					int y = l_bs.BinarySearch(ss[x]);
					if (y >= 0)
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a SortedList... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (sl.ContainsKey(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a Dictionary Key... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (d.ContainsKey(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a Dictionary Value... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (d.ContainsValue(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a SortedDictionary Key... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (sd.ContainsKey(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a SortedDictionary Value... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (sd.ContainsValue(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching ConcurrentDictionary key... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (cd.ContainsKey(ss[x]))
						subtotal += 1;

					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching ConcurrentDictionary value... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (cd.Values.Any(z => z == ss[x]))
						subtotal += 1;

					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting searching a HashSet... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (hs.Contains(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a HashTable Key... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (ht.ContainsKey(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Thread.Sleep(1000); //Sleep to give the system time to recover for next run

			total = 0;
			Console.WriteLine("Starting parallel searching a HashTable Value... ");
			start = DateTime.Now;
			Parallel.For(0, ss.Length,
				() => 0,
				(x, loopState, subtotal) =>
				{
					if (ht.ContainsValue(ss[x]))
					{   //found it.
						subtotal += 1;
					}
					return subtotal;
				},
				(s) =>
				{
					lock (lockObject)
					{
						total += s;
					}
				}
			);
			end = DateTime.Now;
			Console.WriteLine("Finished at: " + end.ToLongTimeString());
			Console.WriteLine("Time: " + (end - start));
			Console.WriteLine("Total finds: " + total + Environment.NewLine);
			Console.WriteLine();
			Console.WriteLine("###########################################################");
			Console.WriteLine();

			Array.Clear(ss, 0, ss.Length);
			ss = null;
			Array.Clear(a, 0, a.Length);
			a = null;
			al.Clear();
			al = null;
			l.Clear();
			l = null;
			l_bs.Clear();
			l_bs = null;
			sl.Clear();
			sl = null;
			d.Clear();
			d = null;
			sd.Clear();
			sd = null;
			cd.Clear();
			cd = null;
			hs.Clear();
			hs = null;
			ht.Clear();
			ht = null;

			GC.Collect();

		} //TestFastestStructureForStringLookup
	}
}