using System.Collections.Generic;
using System;

/*!
  \brief Usefull functions for LinkedList
 */
public static class LinkedListExtensions   
{
  /*!
    \brief Append a IEnumerable at the end of another LinkedList
    \param source The destination list
    \param items The list to append
   */
  public static void AppendRange<T>(this LinkedList<T> source,
                                    IEnumerable<T> items)
  {
    foreach (T item in items)
        source.AddLast(item);
  }

  /*!
    \brief Append a IEnumerable at the end of another LinkedList
    \param source The destination list
    \param items The list to prepend
   */
  public static void PrependRange<T>(this LinkedList<T> source,
                                     IEnumerable<T> items)
  {
    LinkedListNode<T> first = source.First;
    foreach (T item in items)
        source.AddBefore(first, item);
  }
  
  /*!
    \brief Shuffle a LinkedList
    \param list The list to shuffle
   */
  public static void Shuffle<T>(LinkedList<T> list)
  {
    
    Random rand = new Random();

    for (LinkedListNode<T> n = list.First; n != null; n = n.Next)
      {
        T v = n.Value;
        if (rand.Next(0, 2) == 1)
          {
            n.Value = list.Last.Value;
            list.Last.Value = v;
          }
        else
          {
            n.Value = list.First.Value;
            list.First.Value = v;
          }
      }
  }

  public delegate bool Predicate<T>(T t);
  /*!
    \brief Filter a LinkedList using a predicate
    \param list The list to filter
    \param predicate The predicate to filter elements of the list
   */
  public static LinkedList<T> Filter<T>(LinkedList<T> list, Predicate<T> predicate) {
    string predicateString = predicate==null?"(null)":"predicate";
    Logger.Log("LinkedListExtensions::Filter("+Logger.ToString<T>(list)+", "+predicateString+")", Logger.Level.TRACE);
    LinkedList<T> result = new LinkedList<T>();
    foreach (T t in list) {
      if (predicate(t)) {
        result.AddLast(t);
      }
    }
    return result;
  }


  public static T Find<T>(LinkedList<T> list, Predicate<T> predicate) {
    foreach (T t in list) {
      if (predicate(t)) {
        return t;
      }
    }
    Logger.Log("LinkedListExtensions::Find couldn't find any fitting element!", Logger.Level.WARN);
    return default(T);
  }

  public static bool Equals<T>(LinkedList<T> list1, LinkedList<T> list2)
  {
    LinkedListNode<T> it1 = list1.First;
    LinkedListNode<T> it2 = list2.First;

    bool equal = true;

    while(equal && (it1 != null) && (it2 != null))
    {

      equal = (it1.Value.Equals(it2.Value));

      it1 = it1.Next;
      it2 = it2.Next;
    }

    return (equal && (it1 == null) && (it2 == null));
  }
}