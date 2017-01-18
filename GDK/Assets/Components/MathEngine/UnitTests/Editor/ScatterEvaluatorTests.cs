﻿using System.Collections.Generic;
using System.IO;
using GDK.MathEngine;
using GDK.MathEngine.Evaluators;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ScatterEvaluatorTests
{
	private IRng rng;
	private IEvaluator scatterEvaluator;
	private Paytable paytable;

	[TestFixtureSetUp]
	public void Init ()
	{
		paytable = new Paytable ();
		scatterEvaluator = new ScatterEvaluator ();

		PaytableBuilder builder = new ScatterPaytableBuilder ();
		paytable.ReelGroup = builder.BuildReelGroup ();
		paytable.PaylineGroup = builder.BuildPaylineGroup ();
		paytable.PayComboGroup = builder.BuildPayComboGroup ();
		paytable.ScatterComboGroup = builder.BuildScatterComboGroup ();
		paytable.PickTableGroup = builder.BuildPickTableGroup ();
		paytable.PaytableTriggerGroup = builder.BuildPaytableTriggerGroup ();
	}

	[Test]
	public void Evaluation_Initialization ()
	{
		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.IsNotNull (results);
	}

	[Test]
	public void Evaluation_SlotResult1 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(0, "AA"), 3, 1000));

		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (1, results.Results.Count);
	}

	[Test]
	public void Evaluation_SlotResult2 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(1, "BB"), 3, 1000));

		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (1, results.Results.Count);
	}

	[Test]
	public void Evaluation_SlotResult3 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(1, "BB"), 2, 1000));

		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (1, results.Results.Count);
	}

	[Test]
	public void Evaluation_SlotResult4 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(1, "BB"), 4, 1000));

		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (0, results.Results.Count);
	}

	[Test]
	public void Evaluation_SlotResult5 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(0, "AA"), 3, 1000));
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(1, "BB"), 3, 500));
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(2, "CC"), 3, 10));

		// TODO: This will start coming together when we figure out how all the results accumulate.
		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (3, results.Results.Count);
		Assert.AreEqual (1000, results.Results [0].TotalValue);
		Assert.AreEqual (500, results.Results [1].TotalValue);
		Assert.AreEqual (10, results.Results [2].TotalValue);
	}

	[Test]
	public void Evaluation_SlotResult6 ()
	{
		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(0, "AA"), 1, 10));
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(new Symbol(0, "AA"), 2, 100));

		// TODO: This is a bug in the scatter evaluation.
		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (1, results.Results.Count);
		Assert.AreEqual (100, results.Results [0].TotalValue);
	}

	[Test]
	public void Evaluation_SlotResult7 ()
	{
		List<Symbol> combo = new List<Symbol> () {
			new Symbol (0, "AA"),
			new Symbol (1, "BB"),
			new Symbol (2, "CC")
		};

		paytable.ScatterComboGroup.Combos.Clear ();
		paytable.ScatterComboGroup.AddPayCombo (new PayCombo(combo, 150));

		// TODO: This is a bug in the scatter evaluation.
		rng = new DummyRng (new List<int> { 0, 0, 0 });
		SlotResults results = scatterEvaluator.Evaluate (paytable, rng);
		Assert.AreEqual (3, results.Results.Count);
		Assert.AreEqual (150, results.Results [0].TotalValue);
		Assert.AreEqual (150, results.Results [1].TotalValue);
		Assert.AreEqual (150, results.Results [2].TotalValue);
	}
}