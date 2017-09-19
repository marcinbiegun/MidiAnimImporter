﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.Collections.Generic;
using System.IO;

namespace MidiAnim
{
    [ScriptedImporter(1, "midianim")]
    class MidiAnimImporter : ScriptedImporter
    {
        public float _bpm = 120;

        public override void OnImportAsset(AssetImportContext context)
        {
            var song = MidiFileLoader.Load(File.ReadAllBytes(context.assetPath));
            var seq = new MidiTrackSequencer(song.tracks[0], song.division, _bpm);

            var clip = new MidiClip(_bpm);

            clip.WriteBeat(0);
            clip.WriteEvents(0, seq.Start());

            const float dt = 1.0f / 60;
            for (var t = dt; seq.Playing; t += dt)
            {
                clip.WriteBeat(t);
                clip.WriteEvents(t, seq.Advance(dt));
            }

            context.SetMainAsset("MIDI", clip.ConvertToAnimationClip());
        }
    }
}
