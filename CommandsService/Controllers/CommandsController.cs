using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("/api/[controller]/platforms/{platformId:int}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Getting Commands for Platform: {platformId}");

            if (!_commandRepository.PlatformExists(platformId)) return NotFound();

            var commands = _commandRepository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting Command for Platform: {platformId} / {commandId}");

            if (!_commandRepository.PlatformExists(platformId)) return NotFound();

            var command = _commandRepository.GetCommand(platformId, commandId);

            if (command == null) return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId,
            [FromBody] CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Creating Command for Platform: {platformId}");

            if (!_commandRepository.PlatformExists(platformId)) return NotFound();

            var command = _mapper.Map<Command>(commandDto);

            _commandRepository.CreateCommand(platformId, command);
            _commandRepository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new {platformId, commandId = commandReadDto.Id},
                commandReadDto);
        }
    }
}