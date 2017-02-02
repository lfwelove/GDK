﻿using UnityEngine;
using System.Collections.Generic;

using GDK.Pool;
using GDK.MathEngine;
using GDK.Utilities;
using DG.Tweening;
using Zenject;

namespace GDK.Reels
{
	[RequireComponent (typeof(ReelSettings))]
	public class ReelLayout : MonoBehaviour
	{
		[Inject] Paytable paytable;
		[Inject] IRng rng;
		[Inject] ISymbolFactory symbolFactory;

		private List<GameObject> symbolContainers = new List<GameObject> ();
		private List<GameObject> symbolObjects = new List<GameObject> ();

		private ReelSettings settings;

		private Vector3 initialPosition;

		void Start ()
		{
			DOTween.Init (false, true, LogBehaviour.ErrorsOnly);

			settings = GetComponent<ReelSettings> ();
			initialPosition = gameObject.transform.position;

			// Create the symbol containers.
			for (int i = 0; i < settings.VisibleSymbols; ++i)
			{
				var symbolContainer = new GameObject ();
				symbolContainer.name = "SymbolContainer";
				symbolContainer.transform.parent = gameObject.transform;
				symbolContainer.transform.localPosition = Vector3.zero;
				symbolContainers.Add (symbolContainer);
			}

			// Align the symbol containers.
			float reelHeight = (settings.VisibleSymbols - 1) * settings.SymbolSpacing;
			AnimationCurveExtensions.AlignVerticalCenter (
				symbolContainers, 
				initialPosition.y + reelHeight / 2, 
				initialPosition.y - reelHeight / 2);

			// Attach the symbol as a child objects of the symbol containers.
			for (int i = 0; i < settings.VisibleSymbols; ++i)
			{
				var symbolObject = PoolManager.Obtain (symbolFactory.CreateSymbol ("AA"));
				symbolObject.transform.parent = symbolContainers [i].transform;
				symbolObject.transform.localPosition = Vector3.zero;
				symbolObjects.Add (symbolObject);
			}
		}

		private bool spinning;

		private void Update ()
		{
			if (Input.touchCount > 0 || Input.GetKeyDown (KeyCode.Space))
			{
				if (!spinning)
					SetTween ();

				spinning = !spinning;
			}
		}

		private void OnComplete ()
		{
			// Return the last symbol object to the pool.
			PoolManager.Return (symbolObjects [symbolObjects.Count - 1]);

			// Shuffle all symbols down.
			for (int i = symbolObjects.Count - 1; i > 0; --i)
			{
				symbolObjects [i] = symbolObjects [i - 1];
				symbolObjects [i].transform.parent = symbolContainers [i].transform;
				symbolObjects [i].transform.localPosition = Vector3.zero;
			}

			// Pick a random symbol for fun!
			List<ReelProperties> reelProps = paytable.ReelGroup.Reels;
			List<Symbol> symbols = reelProps [0].ReelStrip.Symbols;
			int random = rng.GetRandomNumber (symbols.Count);

			// Add the new symbol object.
			symbolObjects [0] = PoolManager.Obtain (symbolFactory.CreateSymbol (symbols [random].Name));
			symbolObjects [0].transform.parent = symbolContainers [0].transform;
			symbolObjects [0].transform.localPosition = Vector3.zero;

			// Reset the reel mover.
			gameObject.transform.position = initialPosition;

			if (spinning)
				SetTween ();
		}

		private void SetTween ()
		{
			gameObject.transform.DOMove (
				new Vector3 (
					initialPosition.x,
					initialPosition.y - settings.SymbolSpacing, 
					initialPosition.z), settings.SpinTime)
				.SetEase (Ease.Linear)
				.OnComplete (OnComplete);
		}
	}
}