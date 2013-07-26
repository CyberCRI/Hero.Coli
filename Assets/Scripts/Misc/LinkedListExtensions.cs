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
}