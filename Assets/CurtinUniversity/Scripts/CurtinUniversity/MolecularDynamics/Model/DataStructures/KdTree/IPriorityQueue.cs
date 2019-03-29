using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurtinUniversity.MolecularDynamics.Model.DataStructures.KdTree {

    public interface IPriorityQueue<TItem, TPriority>
	{
		void Enqueue(TItem item, TPriority priority);

		TItem Dequeue();

		int Count { get; }
	}
}
