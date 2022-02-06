using AutoMapper;
using Bookstore.API.Dtos;
using Bookstore.API.Helpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : ControllerBase
    {

        private readonly IRepository<Book> _bookRepo;

        private readonly IMapper _mapper;

        public BooksController(IRepository<Book> bookRepo, IMapper mapper)
        {
            _bookRepo = bookRepo;

            _mapper = mapper;
        }

        // GET: api/<BooksController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<BookDto>>> GetBooks([FromQuery] SpecsParams Params)
        {
            if (Params.PageSize<1)
            {
                return BadRequest("Page size can't be less than 1");
            }

            var spec = new BooksWithYearOfPublicationAndAuthor(Params);

            var countSpec = new BooksFiltersForCountSpecification(Params);

            var totalItems = await _bookRepo.CountAsync(countSpec);

            var books = await _bookRepo.ListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<BookDto>>(books);

            return Ok(new Pagination<BookDto>(Params.PageIndex, Params.PageSize, totalItems, data));
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> GetBook(Guid id)
        {
            var spec = new BooksWithYearOfPublicationAndAuthor(id);

            var book = await _bookRepo.GetEntityWithSpec(spec);

            var bookDto = new BookDto();

            var returnValue = _mapper.Map(book, bookDto);

            if (returnValue == null)
            {
                return NotFound(returnValue);
            }

            return Ok(returnValue);
        }

        // POST api/<BooksController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] BookToSaveDto value)
        {

            var newBook = new Book();

            var book = _mapper.Map(value,newBook);

            if (book==null)
            {
                return BadRequest("Problem creating the book");
            }
            try
            {
                _bookRepo.Add(book);

                await _bookRepo.Complete();
            }
            catch (Exception)
            {

            }
           
            return Ok(book);
        }

        // PUT api/<BooksController>/5
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put([FromBody] BookToSaveDto value)
        {

            var newBook = new Book();

            var book = _mapper.Map(value, newBook);

            if (book == null)
            {
                return BadRequest("Problem updating the book");
            }
            try
            {
                _bookRepo.Update(book);

                await _bookRepo.Complete();
            }
            catch (Exception)
            {
                return BadRequest("Problem updating the book");
            }

            return Ok(book);
        }

    }
}
