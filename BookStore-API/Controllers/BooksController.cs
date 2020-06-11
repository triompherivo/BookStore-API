// ***********************************************************************
// Assembly         : BookStore-API
// Author           : RAMANDALAHY TRIOMPHE
// Created          : 06-07-2020
//
// Last Modified By : RAMANDALAHY TRIOMPHE
// Last Modified On : 06-07-2020
// ***********************************************************************
// <copyright file="BooksController.cs" company="BookStore-API">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

/// <summary>
/// Interacts with the Books Table.
/// </summary>
namespace BookStore_API.Controllers
{
    /// <summary>
    /// Class BooksController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IBookRepository _repository;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILoggerService _logger;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public BooksController(IBookRepository repository,ILoggerService logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the books.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                var books = await _repository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successfull");
                return Ok(response);

            }
            catch (Exception ex)
            {

                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }
        /// <summary>
        /// Gets the book.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id: {id}");
                var book = await _repository.FindById(id);
                if(book == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with   id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location}: Successfully got record ²with id: {id}");
                return Ok(response);

            }
            catch (Exception ex)
            {

                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }
        /// <summary>
        /// Creates the specified book dto.
        /// </summary>
        /// <param name="bookDTO">The book dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} : Create Attempted");
                if(bookDTO == null)
                {
                    _logger.LogWarn($"{location} : Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} : data was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _repository.Create(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Creation failed");
                }
                _logger.LogInfo($"{location}: Creation was successful");
                return Created("Create", new { book });
            }
            catch (Exception ex)
            {

                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }

        }

        /// <summary>Updates the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="bookDTO">The book dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id,[FromBody] BookUpdateDTO bookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} : Update Attempted on record with id: {id} ");
                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location} : Update failed with bad data id: {id}");
                    return BadRequest();
                }
                var isExists = await _repository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} : Failed to retrieve record with  id: {id}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} : data was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _repository.Update(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Update failed");
                }
                _logger.LogInfo($"{location}: Update of record with id: {id}  was successful");
                return NoContent();
            }
            catch (Exception ex)
            {

                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }

        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} : delete Attempted on record with id: {id} ");
                if (id < 1 )
                {
                    _logger.LogWarn($"{location} : Delete failed with bad data id: {id}");
                    return BadRequest();
                }
                var isExists = await _repository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} : Failed to retrieve record with  id: {id}");
                    return NotFound();
                }

                var book = await _repository.FindById(id);
                var isSuccess = await _repository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: delete failed for the record with id: {id}");
                }
                _logger.LogInfo($"{location}: Delete of record with id: {id}  was successful");
                return NoContent();
            }
            catch (Exception ex)
            {

                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }

        }

        /// <summary>
        /// Gets the controller action names.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// Internals the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>ObjectResult.</returns>
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Someting went wrong.Please contact the administrator");
        }

    }
}
