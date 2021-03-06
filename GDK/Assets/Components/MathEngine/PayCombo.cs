﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDK.MathEngine
{
	/// <summary>
	/// Represents a pay combination.
	/// </summary>
	[Serializable]
	public class PayCombo
	{
		/// <summary>
		/// Gets the list of symbols in the pay combination.
		/// </summary>
		public List<Symbol> SymbolsInPayCombo { get; set; }

		/// <summary>
		/// Gets the pay amount for this pay combination.
		/// </summary>
		public int PayAmount { get; set; }

		/// <summary>
		/// Paytable item to trigger. TODO: Might even be a list.
		/// </summary>
		public string Trigger { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PayCombo"/> class.
		/// </summary>
		public PayCombo ()
		{
			SymbolsInPayCombo = new List<Symbol> ();
			PayAmount = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PayCombo"/> class.
		/// </summary>
		/// <param name="symbols">The symbols in the pay combination.</param>
		/// <param name="payAmount">The pay amount.</param>
		public PayCombo (List<Symbol> symbols, int payAmount, string trigger = "")
		{
			SymbolsInPayCombo = symbols;
			PayAmount = payAmount;
			Trigger = trigger;
		}

		public PayCombo (Symbol symbol, int count, int payAmount, string trigger = "")
		{
			SymbolsInPayCombo = new List<Symbol> ();
			for (int i = 0; i < count; ++i)
			{
				SymbolsInPayCombo.Add (symbol);
			}
			PayAmount = payAmount;
			Trigger = trigger;
		}

		/// <summary>
		/// Determines whether the given symbols meet the minimum requirements for a match.
		/// </summary>
        /// <remarks>
        /// A PayCombo could be defined as { AA, AA, AA }. In order to match this PayCombo,
        /// the given symbols in the payline must have at least 3 x AA in it.
        /// </remarks>
		/// <returns><c>true</c> if this instance matches the specified symbols; otherwise, <c>false</c>.</returns>
        /// <param name="symbolComparer">Reference to the symbol comparer.</param>
		/// <param name="symbolsInPayline">The symbols to match.</param>
		public bool IsMatch(ISymbolComparer symbolComparer, List<Symbol> symbolsInPayline)
		{
			// Cannot match if the PayCombo requires more symbols than we are given.
			if (SymbolsInPayCombo.Count > symbolsInPayline.Count)
			{
				return false;
			}

			bool match = true;
			for (int i = 0; i < SymbolsInPayCombo.Count; ++i)
			{
                if (!symbolComparer.Match(SymbolsInPayCombo [i], symbolsInPayline [i]))
				{
					match = false;
					break;
				}
			}

			return match;
		}

		public override string ToString()
		{
		    return string.Join(", ", SymbolsInPayCombo.Select(x => x.ToString()).ToArray());
		}
	}

	/// <summary>
	/// Represents a set, or group of pay combinations.
	/// </summary>
	[Serializable]
	public class PayComboGroup
	{
        /// <summary>
        /// Used to compare symbols in the pay combo.
        /// </summary>
        public ISymbolComparer SymbolComparer { get; set; }

		/// <summary>
		/// Gets the paylines in the payline group.
		/// </summary>
		public List<PayCombo> Combos { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PayComboGroup"/> class.
		/// </summary>
        public PayComboGroup(ISymbolComparer symbolComparer)
		{
			Combos = new List<PayCombo> ();
            SymbolComparer = symbolComparer;
		}

		/// <summary>
		/// Adds a pay combo to the pay combo group.
		/// </summary>
		/// <param name="payCombo">The payline to add.</param>
		public void AddPayCombo (PayCombo payCombo)
		{
			Combos.Add (payCombo);
		}
	}
}
