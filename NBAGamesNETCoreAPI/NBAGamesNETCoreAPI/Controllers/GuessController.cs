using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBAGamesNETCoreAPI.DataContexts;
using NBAGamesNETCoreAPI.Models;

namespace NBAGamesNETCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuessController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuessController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Guess
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuessFromAndroid>>> GetAllGuesses()
        {
            return await _context.AllGuesses.ToListAsync();
        }

        // GET: api/Guess/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GuessFromAndroid>> GetGuessFromAndroid(int id)
        {
            var guessFromAndroid = await _context.AllGuesses.FindAsync(id);

            if (guessFromAndroid == null)
            {
                return NotFound();
            }

            return guessFromAndroid;
        }

        // PUT: api/Guess/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuessFromAndroid(int id, GuessFromAndroid guessFromAndroid)
        {
            if (id != guessFromAndroid.ID)
            {
                return BadRequest();
            }

            _context.Entry(guessFromAndroid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuessFromAndroidExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Guess
        [HttpPost]
        public async Task<ActionResult<GuessFromAndroid>> PostGuessFromAndroid(GuessFromAndroid guessFromAndroid)
        {
            if(guessFromAndroid != null)
            {
                _context.AllGuesses.Add(guessFromAndroid);
                await _context.SaveChangesAsync();

                Debug.WriteLine("Good POST request received!");

                return CreatedAtAction("GetGuessFromAndroid", new
                {
                    id = guessFromAndroid.ID,
                    userId = guessFromAndroid.UserId,
                    gameId = guessFromAndroid.GameId,
                    selTeam = guessFromAndroid.SelTeam,
                    byPts = guessFromAndroid.ByPts
                }, guessFromAndroid);
            }

            else
            {
                Debug.WriteLine("Bad POST request received!");
                return BadRequest();
            }      

           /*

            return CreatedAtAction("GetGuessFromAndroid", new { id = guessFromAndroid.ID, 
                                                                userId = guessFromAndroid.UserId, 
                                                                gameId = guessFromAndroid.GameId, 
                                                                selTeam = guessFromAndroid.SelTeam,
                                                                byPts = guessFromAndroid.ByPts}, guessFromAndroid); */
        }

        // DELETE: api/Guess/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GuessFromAndroid>> DeleteGuessFromAndroid(int id)
        {
            var guessFromAndroid = await _context.AllGuesses.FindAsync(id);
            if (guessFromAndroid == null)
            {
                return NotFound();
            }

            _context.AllGuesses.Remove(guessFromAndroid);
            await _context.SaveChangesAsync();

            return guessFromAndroid;
        }

        private bool GuessFromAndroidExists(int id)
        {
            return _context.AllGuesses.Any(e => e.ID == id);
        }
    }
}
