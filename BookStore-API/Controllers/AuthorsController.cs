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
using NLog.LayoutRenderers;

namespace BookStore_API.Controllers
{

    /// <summary>Endpoint used to interact with the authors i nthe Book store's database.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.ControllerBase"/></summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository repository,ILoggerService logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>Gets the authors.</summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        /// 
        [HttpGet]
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempted Get All Authors ");
                var authors = await _repository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully got all authors");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
                
            }
            

        }
        /// <summary>Gets the author.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An Author required</returns>
        [HttpGet("{id}")]
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted Get Author with id: {id}");
                var author = await _repository.FindById(id);
                if(author == null)
                {
                    _logger.LogWarn($"Author with id {id}  was found");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully got Author id : {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
                
            }


        }
        /// <summary>Creates the specified author dto.</summary>
        /// <param name="authorDTO">The author dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [Authorize(Roles ="Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogWarn($"Author submission Attempted");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author data was Incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _repository.Create(author);
                if (!isSuccess)
                {
                    
                    return InternalError($"Author creation failed");
                }
                _logger.LogInfo("Author created");
                return Created("Create", new { author });

            }
            catch (Exception ex)
            {

                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>Updates the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="authorDTO">The author dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id,[FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogWarn($"Author update Attempted - id: {id}");
                if ( id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn("Author updating failed with bad data");
                    return BadRequest();
                }
                var isExists = await _repository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author with id: {id} was not found ");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Author data was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _repository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"updating Author failed");
                }
                _logger.LogWarn($"Author with  id: {id} was successfully updated");
                return NoContent();

            }
            catch (Exception ex)
            {

                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogWarn($"Author deleting Attempted - id: {id}");
            try
            {
                if(id < 1)
                {
                    _logger.LogWarn("Author delete failed with bad data");
                    return BadRequest();
                }
                var isExists = await _repository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author with id: {id} was not found ");
                    return NotFound();
                }
                var author = await _repository.FindById(id);
                var isSuccess = await _repository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"deleting Author failed");
                }
                _logger.LogWarn($"Author with  id: {id} was successfully deleted");
                return NoContent();

            }
            catch (Exception ex)
            {

                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Someting went wrong.Please contact the administrator");
        }

    }
}
