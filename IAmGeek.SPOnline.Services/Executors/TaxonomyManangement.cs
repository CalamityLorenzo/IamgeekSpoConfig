using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services
{
    public class TaxonomyManangement
    {
        private readonly TaxonomySession ts;
        private static TaxonomyManangement _tMan = null;


        private TaxonomyManangement()
        {
            this.ts = SPOConfiguration.GetService<TaxonomySession>();

        }

        public static TaxonomyManangement Manager
        {
            get
            {
                if (_tMan == null)
                {
                    _tMan = new TaxonomyManangement();
                }
                return _tMan;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupDetails">Item1 = Name, Item2 = ID</param>
        public void CreateGroups(IEnumerable<Tuple<string, Guid>> GroupDetails)
        {
            var adminStore = ts.GetDefaultKeywordsTermStore();

            var adminGroups = LoadAdminStore(adminStore);
            // If the group already exists move on.
            // We're eager to please
            foreach (var group in GroupDetails)
            {
                var existingGroup = adminGroups.FirstOrDefault(o => o.Name == group.Item1);
                if (existingGroup != null)
                {
                    Console.WriteLine("Group exists : {0}", group.Item1);
                }
                else
                {
                    Console.WriteLine("Creating : {0}", group.Item1);
                    adminStore.CreateGroup(group.Item1, group.Item2);
                }
            }

            adminStore.CommitAll();

        }

        private TermGroupCollection LoadAdminStore(TermStore adminStore)
        {
            var groups = adminStore.Groups;
            ts.Context.Load(groups, eg => eg.Include(f => f.Name, f => f.Id, f => f.TermSets, f => f.TermSets.Include(tset => tset.Name, tset => tset.Id)));
            ts.Context.ExecuteQuery();
            return groups;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="termSets">Item1 = Group Name, Item2 = Termset Name, Item3 = Termset Id</param>
        public void CreateTermsets(IEnumerable<Tuple<string, string, Guid>> termSets)
        {
            var adminStore = ts.GetDefaultKeywordsTermStore();
            var adminGroups = LoadAdminStore(adminStore);
            // assume the groups already exist.
            foreach (var termSet in termSets)
            {
                var existingGroup = adminGroups.FirstOrDefault(o => o.Name == termSet.Item1);
                var existingTermSet = existingGroup.TermSets.FirstOrDefault(o => o.Name == termSet.Item2);
                if (existingTermSet != null)
                {
                    Console.WriteLine("Term set exists : '{0}'", termSet.Item2);
                }
                else
                {
                    Console.WriteLine("Creating : '{0}'", termSet.Item2);
                    existingGroup.CreateTermSet(termSet.Item2, termSet.Item3, 1033);
                }
            }

            adminStore.CommitAll();
            this.ts.Context.ExecuteQuery();
        }

        /// <summary>
        /// Creates terms at the root of a termset
        /// </summary>
        /// <param name="TermSetId"></param>
        /// <param name="TermInfo"></param>
        public void CreateTermSetTerms(Guid TermSetId, IEnumerable<Tuple<string,Guid>> TermInfo)
        {
            var adminStore = ts.GetDefaultKeywordsTermStore();
            var adminGroups = LoadAdminStore(adminStore);

            var termSet = adminStore.GetTermSet(TermSetId);
            ts.Context.Load(termSet, ts=>ts.Terms, ts=>ts.Terms.Include(o=>o.Id, o=>o.Name));
            ts.Context.ExecuteQuery();
            foreach(var term in TermInfo)
            {
                var existingTerm = termSet.Terms.FirstOrDefault(o => o.Id == term.Item2);
                if (existingTerm == null)
                {
                    Console.WriteLine("Creating term : '{0}'", term.Item1);
                    termSet.CreateTerm(term.Item1, 1033, term.Item2);
                }
                else
                {
                    Console.WriteLine("Term exists: '{0}'", term.Item1);
                }
            }

            ts.Context.ExecuteQuery();

        }

        public void ImportTermsData(IEnumerable<string> pathData, string GroupName, string termSetName)
        {
            var adminGroups = LoadAdminStore(ts.GetDefaultKeywordsTermStore());
            var termSet = adminGroups.FirstOrDefault(o => o.Name == GroupName).TermSets.FirstOrDefault(tName => tName.Name == termSetName);
         //   ProcessTermsetData(pathData, termSet);
            ProcessTermsetDataA(pathData, termSet);
        }

        private void ProcessTermsetDataA(IEnumerable<string> pathData, TermSet termSet)
        {
            foreach (var row in pathData)
            {
                var allTerms = termSet.GetAllTerms();
                ts.Context.Load(allTerms, ax => ax.Include(t => t.Terms, t => t.PathOfTerm, t=>t.IsReused, t=>t.SourceTerm, t => t.IsSourceTerm,  t =>t.IsPinned, t => t.Name, t => t.Id, t => t.Parent, t => t.PathOfTerm, t => t.Parent.Name, t => t.Parent.Id));
                ts.Context.ExecuteQuery();

                var rowCells = row.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                var rowStack = new Stack<Tuple<string, Guid>>();
                for(var cell = rowCells.Length-1; cell >=0 ;--cell)
                {
                    var currentCell = rowCells[cell];
                    // we check the paths and work backwards
                    // for each one we don't have put in LIFO stack
                   var term =  allTerms.FirstOrDefault(o => o.PathOfTerm == String.Join(";", rowCells.Where((cel, idx) => idx < cell+1)));
                   
                   rowStack.Push(Tuple.Create(currentCell, (term!= null)?term.Id:Guid.Empty));
                }

                // We now create any of the missing terms
                CreateStackOTerms(rowStack, allTerms, termSet);
            }
        }

        private void CreateStackOTerms(Stack<Tuple<string, Guid>> rowStack, TermCollection allTerms, TermSet termSet)
        {
            var arrayStack = rowStack.ToArray();
           for(int x = 0; x < arrayStack.Length; ++x)
            {
                var currentItem = arrayStack[x];
                if(currentItem.Item2 == Guid.Empty)
                {
                    var newGuid = Guid.NewGuid();
                    if (x != 0)
                    {
                        var previousTermId = arrayStack[x - 1].Item2;
                        var previousTerm = allTerms.FirstOrDefault(o=>o.Id == previousTermId);
                        //termSet.Context.Load(previousTerm);
                       
                        if (previousTerm.IsReused && previousTerm.IsSourceTerm == false)
                        {
                            // get term
                            var sourceTerm = previousTerm.SourceTerm;
                            termSet.Context.Load(sourceTerm, sc=>sc.Id, sc => sc.Terms, sc => sc.Terms.Include(t => t.Name, t => t.Id));
                            termSet.Context.ExecuteQuery();

                            var sourceNewTerm = sourceTerm.Terms.FirstOrDefault(trm => trm.Name == currentItem.Item1);

                            // not in the original either
                            if (sourceNewTerm == null)
                            {
                                // add new term
                                var newTerm = sourceTerm.CreateTerm(currentItem.Item1, 1033, newGuid);
                                // reuse it in the term-set we are iterating
                                previousTerm.ReuseTerm(newTerm, false);
                            }
                            else{
                                previousTerm.ReuseTerm(sourceNewTerm, false);
                                newGuid = sourceNewTerm.Id;
                            }
                            
                        }
                        else
                        {
                            previousTerm.CreateTerm(currentItem.Item1, 1033, newGuid);
                        }

                            
                    }
                    else
                    {
                        // Create a new term at the root of the termset
                        termSet.CreateTerm(currentItem.Item1, 1033, newGuid);
                    }
                    arrayStack[x] = Tuple.Create(currentItem.Item1, newGuid);

                    allTerms = termSet.GetAllTerms();
                    termSet.Context.Load(allTerms, ax => ax.Include(t => t.Terms, t => t.PathOfTerm, t => t.IsReused, t => t.SourceTerm, t => t.IsSourceTerm, t => t.IsPinned, t => t.Name, t => t.Id, t => t.Parent, t => t.PathOfTerm, t => t.Parent.Name, t => t.Parent.Id));
                    termSet.Context.ExecuteQuery();


                }
            }
        }

        private Term CheckForReusePinning(Term previousTerm)
        {
            if (previousTerm.IsReused && previousTerm.IsSourceTerm == false)
            {
                //Get the original
                return  previousTerm.SourceTerm;
            }
            else
            {
                return previousTerm;
            }
        }

        private void ProcessTermsetData(IEnumerable<string> pathData, TermSet termSet)
        {
            var allTerms = termSet.GetAllTerms();
            ts.Context.Load(allTerms, ax => ax.Include(t => t.Terms, t => t.PathOfTerm, t => t.Name, t => t.Id, t => t.Parent, t => t.PathOfTerm, t => t.Parent.Name, t => t.Parent.Id));
            ts.Context.ExecuteQuery();

            foreach (var row in pathData)
            {
                var rowCells = row.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                for (var cell = 0; cell < rowCells.Length; ++cell)
                {
                    // Can't find a particular term
                    var matchedTerm = allTerms.Where(o => o.Name == rowCells[cell]);
                    if (matchedTerm.Count() == 0)
                    {
                        // If we are on the first entry of arow
                        // create term at the root
                        if (cell == 0)
                        {
                            termSet.CreateTerm(rowCells[cell], 1033, Guid.NewGuid());
                            ts.Context.ExecuteQuery();
                            ts.Context.Load(allTerms);
                            ts.Context.ExecuteQuery();
                        }
                        else
                        {

                            // find the parent by the current row path.
                            // We do this because terms can appear twice if didn't areas eg Budget->2012->Docs vs Budget->2010->Docs
                            // Path is unique
                            var currentPath = String.Join(";", rowCells.Where((cel, idx) => idx < cell));
                            try
                            {
                              
                                var parentTerm = allTerms.First(o => o.PathOfTerm == currentPath);
                                parentTerm.CreateTerm(rowCells[cell], 1033, Guid.NewGuid());
                                ts.Context.ExecuteQuery();
                                ts.Context.Load(allTerms);
                                ts.Context.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message + " \r\n" + ex.StackTrace);
                            }
                        }
                    }
                    termSet.TermStore.CommitAll();
                }
            }




        }

        
    }
}
