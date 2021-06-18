using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Karno
{
    public class KMap
    {
        
        SortedSet<string> on_set_binary;
        SortedSet<string> dc_set_binary;

        public KMap(int number_of_variables, HashSet<long> on_set, HashSet<long> dc_set)
        {
            if (on_set.Intersect(dc_set).Count() > 0)
                throw new ArgumentException("El conjunto ON y el DC deben estar separados");

            if (on_set.Count == 0)
                throw new ArgumentException("El set ON no puede estar vacío");

            NumberOfVariables = number_of_variables;
            ONSet = on_set;
            DCSet = dc_set;

            on_set_binary = new SortedSet<string>(ONSet.Select(e => e.ToBinaryString(number_of_variables)));
            dc_set_binary = new SortedSet<string>(DCSet.Select(e => e.ToBinaryString(number_of_variables)));
        }

        public int NumberOfVariables { get; private set; }

        public HashSet<long> ONSet { get; set; }

        public HashSet<long> DCSet { get; set; }

        public HashSet<Coverage> Minimize()
        {
            var groups = new Coverage();

            // Generar grupos iniciales (de cardinalidad 1)
            foreach (var one in on_set_binary)
                groups.Add(new Group() { one });

            // Considere el no importa como 'unos', pero no los considere esenciales
            foreach (var dc in dc_set_binary)
                groups.Add(new Group() { dc });

            Coverage previous_groups = null;
            // Continuar fusionando y optimizando hasta que sea posible
            while (previous_groups == null || !groups.Equals(previous_groups))
            {
                previous_groups = groups; // Guarde el estado antes de fusionar y optimizar

                // Fusionar grupos de cardinalidad 'n' para crear grupos de doble cardinalidad.
                groups = MergeGroups(groups);
                // Limpiar grupos que sean subconjuntos estrictos de otros grupos.
                groups = RemoveSubsets(groups);
            }

            // Detectar grupos esenciales.
            groups = MarkEssential(groups);

            // Eliminar los grupos completamente redundantes, es decir, los grupos que cubren 'unos' (de la función) ya cubiertos por otros grupos ESENCIALES
            groups = RemoveRedundant(groups);

            return GetCoverages(groups);
        }

        Coverage MergeGroups(Coverage groups)
        {
            var merged = new Coverage();
            foreach (var g1 in groups)
            {
                foreach (var g2 in groups)
                {
                    // Dos grupos solo pueden fusionarse si son adyacentes y separados
                    if (g1.Intersect(g2).Count() == 0 && AreGroupsAdjacent(g1, g2))
                    {
                        var new_group = new Group(g1.Union(g2));
                        merged.Add(new_group);
                    }
                }
            }

            return new Coverage(groups.Union(merged));
        }

        bool AreGroupsAdjacent(Group group1, Group group2)
        {
            // Para ser adyacentes, necesitan tener la misma cardinalidad
            if (group1.Count != group2.Count)
                return false;

            // Para cada uno en el primer grupo, debe haber un 'emparejamiento' uno en el otro de modo que tengan una distancia de hamming de 1.
            var matched_in_group2 = new Group();
            foreach (var term1 in group1)
            {
                var matched = false;
                foreach (var term2 in group2)
                {

                    if (matched_in_group2.Contains(term2))
                        // Este ya ha sido emparejado previamente, omítelo.
                        continue;
                    else if (Utils.Hamming(term1, term2) == 1)
                    {
                        matched = true;
                        matched_in_group2.Add(term2);
                        break;
                    }
                }

                if (!matched)
                    // Si hay incluso un "uno" en el primer grupo que no tiene un par coincidente, entonces los dos grupos no pueden ser adyacentes
                    return false;
            }

            return true;
        }

        HashSet<Coverage> GetCoverages(Coverage groups)
        {
            // Navegue por la gráfica de (posibles) soluciones, cada una de las cuales incluye o excluye un grupo en particular Cada una de las soluciones es válida (es decir, cubre todas las "unidades")
            var essential = new Coverage(groups.Where(g => g.IsEssential.Value));
            var available_groups_list = groups.Except(essential).OrderBy(g => g.Count);
            return new HashSet<Coverage>(NavigateCoverages(essential, available_groups_list, true));
        }

        IEnumerable<Coverage> NavigateCoverages(Coverage selected_groups, IEnumerable<Group> available_groups_list, bool check_coverage)
        {

            var solutions = new List<Coverage>();

            if (check_coverage && IsValidCoverage(selected_groups, out Coverage coverage))
            {
                solutions.Add(coverage);
            }

            // solución base para recursión
            if (!available_groups_list.Any())
                return solutions;

            var left_branch_groups = new Coverage(selected_groups) { available_groups_list.First() };
            var right_branch_groups = new Coverage(selected_groups);

            var next_available_groups_list = available_groups_list.Skip(1);
            var left_branch_coverages = NavigateCoverages(left_branch_groups, next_available_groups_list, true);
            var right_branch_coverages = NavigateCoverages(right_branch_groups, next_available_groups_list, false);

            return solutions.Union(left_branch_coverages).Union(right_branch_coverages);
        }

        bool IsValidCoverage(Coverage selected_groups, out Coverage coverage)
        {
            coverage = null;
            var on_set = new SortedSet<string>(on_set_binary);

            foreach (var g in selected_groups)
                on_set.ExceptWith(g);

            // Una cobertura es válida si y solo si cubre todos los "unos" de la función
            if (on_set.Any())
                return false;
            else
            {
                coverage = new Coverage(selected_groups) { Cost = CoverageCost(selected_groups) };
                return true;
            }
        }

        private long CoverageCost(Coverage selected_groups)
        {
            //El número de literales en el término mínimo producido por un grupo de cardinalidad 2 ^ n es N - n, donde N es el número de variables de la función booleana para minimizar
            long cost = 0;
            foreach (var g in selected_groups)
            {
                var n = (long)Math.Log(g.Count, 2);
                cost += (NumberOfVariables - n);
            }
            return cost;
        }

        Coverage RemoveSubsets(Coverage groups)
        {
            var not_subsets = new Coverage();
            foreach (var candidate_subset in groups)
            {
                if (!groups.Any(candidate_superset => candidate_subset.IsProperSubsetOf(candidate_superset)))
                    not_subsets.Add(candidate_subset);
            }

            return not_subsets;
        }

        Coverage MarkEssential(Coverage groups)
        {
            // Si hay algún "uno" no cubierto por ningún otro grupo, entonces "g1" es un grupo esencial
            foreach (var g1 in groups)
            {
                // Compare los términos de este grupo con todos los otros grupos y elimine aquellos cubiertos por los otros grupos
                var remaining_terms = new Group(g1.Except(dc_set_binary));
                foreach (var g2 in groups)
                {
                    if (!g1.Equals(g2))
                        remaining_terms.ExceptWith(g2);
                }

                // Si solo hay un término cubierto por SOLO este grupo, es esencial
                if (remaining_terms.Count > 0)
                    g1.IsEssential = true;
                else
                    g1.IsEssential = false;
            }

            return groups;
        }

        Coverage RemoveRedundant(Coverage groups)
        {
            // Un grupo es redundante si cubre "unos" ya cubiertos por otros grupos esenciales
            var redundant = new Coverage();
            var essential = new Coverage(groups.Where(g => g.IsEssential.Value));

            foreach (var g in groups)
            {
                if (g.IsEssential.Value)
                    continue;

                var remaining_terms = new Group(g);
                foreach (var e in essential)
                    remaining_terms.ExceptWith(e);
                if (remaining_terms.Count == 0)
                    redundant.Add(g);
            }

            return new Coverage(groups.Except(redundant));
        }

    }
}
