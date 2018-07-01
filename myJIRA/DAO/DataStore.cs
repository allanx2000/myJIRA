using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myJIRA.DAO
{
    public interface DataStore
    {
        List<JIRAItem> LoadOpenJIRAs();
        List<JIRAItem> LoadArchivedJIRAs(int archivedYear);

        void UpsertJIRA(JIRAItem jira);
        void DeleteJIRA(int id);

        /// <summary>
        /// Returns the Boards in order
        /// </summary>
        /// <returns></returns>
        List<BoardName> GetBoards();

        /// <summary>
        /// Upserts the the list boards
        /// </summary>
        /// <param name="boards"></param>
        void UpsertBoards(List<BoardName> boards);
    }
}
