using System.Collections.Generic;
using System.Linq;

namespace Karno
{
    public class Group : SortedSet<string>
    {

        public Group(IEnumerable<string> collection) : base(collection)
        {

        }

        public Group() : base()
        {

        }

        public bool? IsEssential { get; set; }

        #region Comparacion de igualdad

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;

            var other = (Group)obj;
            return this.SequenceEqual(other);
        }

        public override int GetHashCode()
        {
            // Esta es la razón por la que usamos una lista ordenada para el objeto "Grupo", porque necesitamos crear tantos cubos
            //  como haya primeros elementos distintos de cada grupo.
            if (Count > 0)
                return this.First().GetHashCode();
            else
                return GetHashCode();
        }

        #endregion

    }

}
