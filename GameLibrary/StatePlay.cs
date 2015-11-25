﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StateMachine;

namespace GameLibrary
{
    /// <summary>
    /// StatePlay.
    /// </summary>
    public class StatePlay : BaseState
    {
        /// <summary>
        /// Configure the state in the given state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        public override void Configure(GameStateMachine stateMachine)
        {
            stateMachine.StateMachine.Configure("StatePlay")
                .OnEntry(OnEntry)
                .OnExit(OnExit)
                .Permit("TriggerStateGameOver", "StateGameOver");
        }

        /// <summary>
        /// Entry method to the state.
        /// </summary>
        private void OnEntry()
        {
            Console.WriteLine("OnEntry StatePlay");
        }

        /// <summary>
        /// Exit method from the state.
        /// </summary>
        private void OnExit()
        {
            Console.WriteLine("OnExit StatePlay");
        }
    }
}