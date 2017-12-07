using System;
using System.Collections.Generic;
using ReactiveObjects.Reactive;

namespace ReactiveObjects.Extensions
{
    public static class ReactiveExtensions
    {
        #region List

        public static void Add<T>(this R<List<T>> list, T value) {
            list.Value.Add(value);
            list.Set(list.Value);
        }

        public static void AddRange<T>(this R<List<T>> list, IEnumerable<T> collection) {
            list.Value.AddRange(collection);
            list.Set(list.Value);
        }

        public static void Remove<T>(this R<List<T>> list, T value) {
            list.Value.Remove(value);
            list.Set(list.Value);
        }

        public static void RemoveAll<T>(this R<List<T>> list, Predicate<T> predicate) {
            list.Value.RemoveAll(predicate);
            list.Set(list.Value);
        }

        public static void RemoveAt<T>(this R<List<T>> list, int index) {
            list.Value.RemoveAt(index);
            list.Set(list.Value);
        }

        public static void RemoveRange<T>(this R<List<T>> list, int index, int count) {
            list.Value.RemoveRange(index, count);
            list.Set(list.Value);
        }

        public static void Clear<T>(this R<List<T>> list) {
            list.Value.Clear();
            list.Set(list.Value);
        }

        public static void Insert<T>(this R<List<T>> list, int index, T value) {
            list.Value.Insert(index, value);
            list.Set(list.Value);
        }

        public static void InsertRange<T>(this R<List<T>> list, int index, IEnumerable<T> collection) {
            list.Value.InsertRange(index, collection);
            list.Set(list.Value);
        }

        public static void Reverse<T>(this R<List<T>> list) {
            list.Value.Reverse();
            list.Set(list.Value);
        }

        public static void Reverse<T>(this R<List<T>> list, int index, int count) {
            list.Value.Reverse(index, count);
            list.Set(list.Value);
        }

        public static void Sort<T>(this R<List<T>> list) {
            list.Value.Sort();
            list.Set(list.Value);
        }

        public static void Sort<T>(this R<List<T>> list, IComparer<T> comparer) {
            list.Value.Sort(comparer);
            list.Set(list.Value);
        }

        public static void Sort<T>(this R<List<T>> list, Comparison<T> comparison) {
            list.Value.Sort(comparison);
            list.Set(list.Value);
        }

        public static void Sort<T>(this R<List<T>> list, int index, int count, IComparer<T> comparer) {
            list.Value.Sort(index, count, comparer);
            list.Set(list.Value);
        }

        #endregion

        #region LinkedList

        public static void AddAfter<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node, LinkedListNode<T> newNode) {
            linkedList.Value.AddAfter(node, newNode);
            linkedList.Set(linkedList.Value);
        }

        public static void AddAfter<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node, T value) {
            linkedList.Value.AddAfter(node, value);
            linkedList.Set(linkedList.Value);
        }

        public static void AddBefore<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node, LinkedListNode<T> newNode) {
            linkedList.Value.AddBefore(node, newNode);
            linkedList.Set(linkedList.Value);
        }

        public static void AddBefore<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node, T value) {
            linkedList.Value.AddBefore(node, value);
            linkedList.Set(linkedList.Value);
        }

        public static void AddFirst<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node) {
            linkedList.Value.AddFirst(node);
            linkedList.Set(linkedList.Value);
        }

        public static void AddFirst<T>(this R<LinkedList<T>> linkedList, T value) {
            linkedList.Value.AddFirst(value);
            linkedList.Set(linkedList.Value);
        }

        public static void AddLast<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node) {
            linkedList.Value.AddLast(node);
            linkedList.Set(linkedList.Value);
        }

        public static void AddLast<T>(this R<LinkedList<T>> linkedList, T value) {
            linkedList.Value.AddLast(value);
            linkedList.Set(linkedList.Value);
        }

        public static void Clear<T>(this R<LinkedList<T>> linkedList) {
            linkedList.Value.Clear();
            linkedList.Set(linkedList.Value);
        }

        public static void Remove<T>(this R<LinkedList<T>> linkedList, T value) {
            linkedList.Value.Remove(value);
            linkedList.Set(linkedList.Value);
        }

        public static void Remove<T>(this R<LinkedList<T>> linkedList, LinkedListNode<T> node) {
            linkedList.Value.Remove(node);
            linkedList.Set(linkedList.Value);
        }

        public static void RemoveFirst<T>(this R<LinkedList<T>> linkedList) {
            linkedList.Value.RemoveFirst();
            linkedList.Set(linkedList.Value);
        }

        public static void RemoveLast<T>(this R<LinkedList<T>> linkedList) {
            linkedList.Value.RemoveLast();
            linkedList.Set(linkedList.Value);
        }

        #endregion

        #region Stack

        public static void Push<T>(this R<Stack<T>> stack, T value) {
            stack.Value.Push(value);
            stack.Set(stack.Value);
        }

        public static T Pop<T>(this R<Stack<T>> stack) {
            var value = stack.Value.Pop();
            stack.Set(stack.Value);

            return value;
        }

        public static void Clear<T>(this R<Stack<T>> stack) {
            stack.Value.Clear();
            stack.Set(stack.Value);
        }

        #endregion

        #region Queue

        public static void Enqueue<T>(this R<Queue<T>> queue, T value) {
            queue.Value.Enqueue(value);
            queue.Set(queue.Value);
        }

        public static T Dequeue<T>(this R<Queue<T>> queue) {
            var value = queue.Value.Dequeue();
            queue.Set(queue.Value);

            return value;
        }

        public static void Clear<T>(this R<Queue<T>> queue) {
            queue.Value.Clear();
            queue.Set(queue.Value);
        }

        #endregion

        #region Dictionary

        public static void Add<TK, TV>(this R<Dictionary<TK, TV>> dictionary, TK key, TV value) {
            dictionary.Value.Add(key, value);
            dictionary.Set(dictionary.Value);
        }

        public static void Remove<TK, TV>(this R<Dictionary<TK, TV>> dictionary, TK key) {
            dictionary.Value.Remove(key);
            dictionary.Set(dictionary.Value);
        }

        public static void Clear<TK, TV>(this R<Dictionary<TK, TV>> dictionary) {
            dictionary.Value.Clear();
            dictionary.Set(dictionary.Value);
        }

        #endregion

        #region SortedList

        public static void Add<TK, TV>(this R<SortedList<TK, TV>> sortedList, TK key, TV value) {
            sortedList.Value.Add(key, value);
            sortedList.Set(sortedList.Value);
        }

        public static void Remove<TK, TV>(this R<SortedList<TK, TV>> sortedList, TK key, TV value) {
            sortedList.Value.Add(key, value);
            sortedList.Set(sortedList.Value);
        }

        public static void RemoveAt<TK, TV>(this R<SortedList<TK, TV>> sortedList, int index) {
            sortedList.Value.RemoveAt(index);
            sortedList.Set(sortedList.Value);
        }

        public static void Clear<TK, TV>(this R<SortedList<TK, TV>> sortedList) {
            sortedList.Value.Clear();
            sortedList.Set(sortedList.Value);
        }

        #endregion

        #region HashSet

        public static void Add<T>(this R<HashSet<T>> hashSet, T value) {
            hashSet.Value.Add(value);
            hashSet.Set(hashSet.Value);
        }

        public static void Remove<T>(this R<HashSet<T>> hashSet, T value) {
            hashSet.Value.Remove(value);
            hashSet.Set(hashSet.Value);
        }

        public static void SymmetricExceptWith<T>(this R<HashSet<T>> hashSet, IEnumerable<T> other) {
            hashSet.Value.SymmetricExceptWith(other);
            hashSet.Set(hashSet.Value);
        }

        public static void UnionWith<T>(this R<HashSet<T>> hashSet, IEnumerable<T> other) {
            hashSet.Value.UnionWith(other);
            hashSet.Set(hashSet.Value);
        }

        public static void RemoveWhere<T>(this R<HashSet<T>> hashSet, Predicate<T> predicate) {
            hashSet.Value.RemoveWhere(predicate);
            hashSet.Set(hashSet.Value);
        }

        public static void Clear<T>(this R<HashSet<T>> hashSet) {
            hashSet.Value.Clear();
            hashSet.Set(hashSet.Value);
        }

        #endregion
    }
}