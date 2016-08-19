//-----------------------------------------------------------------------
// <copyright file="IParent.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines the interface that must be implemented</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the interface that must be implemented
  /// by any business object that contains child
  /// objects.
  /// </summary>
  public interface IParent
  {
    /// <summary>
    /// This method is called by a child object when it
    /// wants to be removed from the collection.
    /// </summary>
    /// <param name="child">The child object to remove.</param>
    void RemoveChild(Core.IEditableBusinessObject child);
    /// <summary>
    /// Provide access to the parent reference for use
    /// in child object code.
    /// </summary>
    /// <remarks>
    /// This value will be Nothing for root objects and DynamicLists.
    /// </remarks>
    IParent Parent { get; }
  }
}