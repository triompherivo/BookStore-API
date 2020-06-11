// ***********************************************************************
// Assembly         : BookStore-API
// Author           : RAMANDALAHY TRIOMPHE
// Created          : 06-07-2020
//
// Last Modified By : RAMANDALAHY TRIOMPHE
// Last Modified On : 06-07-2020
// ***********************************************************************
// <copyright file="BookRepository.cs" company="BookStore-API">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    /// <summary>
    /// Class BookRepository.
    /// Implements the <see cref="BookStore_API.Contracts.IBookRepository" />
    /// </summary>
    /// <seealso cref="BookStore_API.Contracts.IBookRepository" />
    public class BookRepository : IBookRepository
    {
        /// <summary>
        /// The database
        /// </summary>
        private readonly ApplicationDbContext _db;
        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Create(Book entity)
        {
            await _db.Books.AddAsync(entity);
            return await Save();
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Delete(Book entity)
        {
            _db.Books.Remove(entity);
            return await Save();
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns>Task&lt;List&lt;Book&gt;&gt;.</returns>
        public async Task<List<Book>> FindAll()
        {
            return await _db.Books.ToListAsync();
        }

        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Book&gt;.</returns>
        public async Task<Book> FindById(int id)
        {
            return await _db.Books.FindAsync(id);
        }

        /// <summary>
        /// Determines whether the specified identifier is exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> IsExists(int id)
        {
            return  await _db.Books.AnyAsync(q => q.Id == id);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> Update(Book entity)
        {
            _db.Update(entity);
            return await Save();
        }
    }
}
