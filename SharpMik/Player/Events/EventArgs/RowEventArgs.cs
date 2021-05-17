using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMik.Player.Events.EventArgs
{
    public class RowEventArgs
    {
		public Module mod;
		static munitrk s_UniTrack = new munitrk();

        public Row Row;

		public Row Row1;
        public Row Row2;
        public Row Row3;
        public Row Row4;
        public Row Row5;

        public RowEventArgs(Row row)
        {
            Row = row;
        }

        public RowEventArgs(Module module)
        {
			mod = module;

            int posPatternRow = module.sngpos;
            int posPattern = module.patpos;


            var numOne = posPatternRow - 2;
            var numTwo = posPatternRow - 1;
            var numThree = posPatternRow;
            var numFour = posPatternRow + 1;
            var numFive = posPatternRow + 2;

            if(numOne < 0)
            {
                if (posPattern > 0)
                {
                    numOne = mod.numrow - numOne;
                }
                else
                {
                    numOne = 0;
                }
            }
            if (numTwo < 0)
            {
                if (posPattern > 0)
                {
                    numTwo = mod.numrow - numTwo;
                }
                else
                {
                    numTwo = 0;
                }
            }
            if (numFour > mod.numrow)
            {
                if (posPattern < mod.numpat)
                {
                    posPattern += 1;

                    if (numFour == 64)
                    {
                        numFour = 0;
                    }
                    if (numFour == 65)
                    {
                        numFour = 1;
                    }
                }
                else
                {
                    numFour = 0;
                }
            }
            if (numFive > mod.numrow)
            {
                if (posPattern < mod.numpat)
                {
                    posPattern += 1;

                    if (numFive == 64)
                    {
                        numFive = 0;
                    }
                    if (numFive == 65)
                    {
                        numFive = 1;
                    }
                }
                else
                {
                    numFive = 0;
                }
            }


            if (posPattern == 0 && posPatternRow == 0)
            {
                Row1 = GenerateFakeRow();
                Row2 = GenerateFakeRow();
                Row3 = GenerateRow(numThree, 0);
                Row4 = GenerateRow(numFour, 0);
                Row5 = GenerateRow(numFive, 0);
            }
            else if (posPattern == 0 && posPatternRow == 1)
            {
                Row1 = GenerateFakeRow();
                Row2 = GenerateRow(numTwo, 0);
                Row3 = GenerateRow(numThree, 0);
                Row4 = GenerateRow(numFour, 0);
                Row5 = GenerateRow(numFive, 0);
            }
            else if (posPattern == mod.numpat && posPatternRow == mod.numrow - 1)
            {
                Row1 = GenerateRow(numOne, posPattern);
                Row2 = GenerateRow(numTwo, posPattern);
                Row3 = GenerateRow(numThree, posPattern);
                Row4 = GenerateRow(numFour, posPattern);
                Row5 = GenerateFakeRow();
            }
            else if (posPattern == mod.numpat && posPatternRow == mod.numrow)
            {
                Row1 = GenerateRow(numOne, posPattern);
                Row2 = GenerateRow(numTwo, posPattern);
                Row3 = GenerateRow(numThree, posPattern);
                Row4 = GenerateFakeRow();
                Row5 = GenerateFakeRow();
            }
            else
            {
                Row1 = GenerateRow(numOne, posPattern);
                Row2 = GenerateRow(numTwo, posPattern);
                Row3 = GenerateRow(numThree, posPattern);
                Row4 = GenerateRow(numFour, posPattern);
                Row5 = GenerateRow(numFive, posPattern);
            }

            //Row3 = GenerateRow(posPatternRow, posPattern);

            Console.WriteLine($"sngps = {module.sngpos}, patos = {module.patpos}");

            //if (pos == 0)
            //{
            //    Row1 = GenerateFakeRow();
            //    Row2 = GenerateFakeRow();
            //    Row3 = GenerateRow(pos, patternPos);
            //    //Row4 = GenerateRow(pos + 1, patternPos);
            //    //Row5 = GenerateRow(pos + 2, patternPos);
            //}
            //else if (pos == 1)
            //{
            //    Row1 = GenerateFakeRow();
            //}
        }

        public Row GenerateFakeRow()
        {
            var row = new Row();
            for (var channel = 0; channel < mod.numchn; channel++)
            {
                var col = new Col();
                row.Cols.Add(col);
            }

            return row;
        }

        public Row GenerateRow(int intPos, int intPatternPos)
		{
            short pos = Convert.ToInt16(intPos);
            ushort patternpos = Convert.ToUInt16(intPatternPos);

            var row = new Row();
			short channel;
			MP_CONTROL a;
			byte c, inst;
			int tr, funky; /* funky is set to indicate note or instrument change */


			for (channel = 0; channel < mod.numchn; channel++)
			{
                var col = new Col();

                a = mod.control[channel];

                //if (pos >= mod.numpos)
                //{
                //    tr = mod.numtrk;
                //    //mod.numrow = 0;
                //}
                //else
                //{
                //    tr = mod.patterns[(mod.positions[pos] * mod.numchn) + channel];
                //    //mod.numrow = mod.pattrows[mod.positions[mod.sngpos]];
                //}

                //if (tr < mod.numtrk)
                //{
                //    int place = s_UniTrack.UniFindRow(mod.tracks[tr], patternpos);
                //    a.row = mod.tracks[tr];
                //    a.rowPos = place;
                //}
                //else
                //{
                //    a.row = null;
                //}

                a.newsamp = 0;
                if (mod.vbtick == 0)
                    a.main.notedelay = 0;

                if (a.row == null)
                    continue;

                s_UniTrack.UniSetRow(a.row, a.rowPos);
                funky = 0;

                while ((c = s_UniTrack.UniGetByte()) != 0)
                {
                    switch (c)
                    {
                        case (byte)SharpMikCommon.Commands.UNI_NOTE:
                            {
                                funky |= 1;
                                a.oldnote = a.anote;
                                a.anote = s_UniTrack.UniGetByte();
                                a.main.kick = SharpMikCommon.KICK_NOTE;
                                a.main.start = -1;
                                a.sliding = 0;

                                /* retrig tremolo and vibrato waves ? */
                                if (!((a.wavecontrol & 0x80) == 0x80))
                                    a.trmpos = 0;

                                if (!((a.wavecontrol & 0x08) == 0x08))
                                    a.vibpos = 0;

                                if (a.panbwave == 0)
                                    a.panbpos = 0;

                                break;
                            }

                        case (byte)SharpMikCommon.Commands.UNI_INSTRUMENT:
                            {

                                inst = s_UniTrack.UniGetByte();
                                if (inst >= mod.numins)
                                    break; /* safety valve */

                                funky |= 2;
                                a.main.i = (mod.flags & SharpMikCommon.UF_INST) == SharpMikCommon.UF_INST ? mod.instruments[inst] : null;
                                a.retrig = 0;
                                a.s3mtremor = 0;
                                a.ultoffset = 0;
                                a.main.sample = inst;
                                break;
                            }


                        default:
                            {
                                s_UniTrack.UniSkipOpcode();
                                break;
                            }
                    }
                }

                if (funky != 0)
                {
                    INSTRUMENT i;
                    SAMPLE s;

                    if ((i = a.main.i) != null)
                    {
                        if (i.samplenumber[a.anote] >= mod.numsmp) continue;
                        s = mod.samples[i.samplenumber[a.anote]];
                        a.main.note = i.samplenote[a.anote];
                    }
                    else
                    {
                        a.main.note = a.anote;
                        s = mod.samples[a.main.sample];
                    }

                    if (a.main.s != s)
                    {
                        a.main.s = s;
                        a.newsamp = a.main.period;
                    }

                    col.note = Convert.ToInt32(a.anote).ToString();
                    col.name = s.samplename;

                    /* channel or instrument determined panning ? */
                    a.main.panning = (short)mod.panning[channel];
                    if ((s.flags & SharpMikCommon.SF_OWNPAN) == SharpMikCommon.SF_OWNPAN)
                        a.main.panning = s.panning;
                    else if ((i != null) && (i.flags & SharpMikCommon.IF_OWNPAN) == SharpMikCommon.IF_OWNPAN)
                        a.main.panning = i.panning;

                    a.main.handle = s.handle;
                    a.speed = s.speed;

                    if (i != null)
                    {
                        if ((mod.panflag) && (i.flags & SharpMikCommon.IF_PITCHPAN) == SharpMikCommon.IF_PITCHPAN
                            && (a.main.panning != SharpMikCommon.PAN_SURROUND))
                        {
                            a.main.panning +=
                                (short)(((a.anote - i.pitpancenter) * i.pitpansep) / 8);
                            if (a.main.panning < SharpMikCommon.PAN_LEFT)
                                a.main.panning = SharpMikCommon.PAN_LEFT;
                            else if (a.main.panning > SharpMikCommon.PAN_RIGHT)
                                a.main.panning = SharpMikCommon.PAN_RIGHT;
                        }
                        a.main.pitflg = i.pitflg;
                        a.main.volflg = i.volflg;
                        a.main.panflg = i.panflg;
                        a.main.nna = i.nnatype;
                        a.dca = i.dca;
                        a.dct = i.dct;
                    }
                    else
                    {
                        a.main.pitflg = a.main.volflg = a.main.panflg = 0;
                        a.main.nna = a.dca = 0;
                        a.dct = SharpMikCommon.DCT_OFF;
                    }

                    if ((funky & 2) == 2) /* instrument change */
                    {
                        /* IT random volume variations: 0:8 bit fixed, and one bit for
                sign. */
                        a.volume = a.tmpvolume = s.volume;
                        if ((s != null) && (i != null))
                        {
                            if (i.rvolvar != 0)
                            {
                                a.volume = a.tmpvolume = (short)(s.volume + (byte)((s.volume * ((int)i.rvolvar * ModPlayer.getrandom(512))) / 25600));

                                if (a.volume < 0)
                                    a.volume = a.tmpvolume = 0;
                                else if (a.volume > 64)
                                    a.volume = a.tmpvolume = 64;
                            }
                            if ((mod.panflag) && (a.main.panning != SharpMikCommon.PAN_SURROUND))
                            {
                                a.main.panning += (short)(((a.main.panning * ((int)i.rpanvar * ModPlayer.getrandom(512))) / 25600));
                                if (a.main.panning < SharpMikCommon.PAN_LEFT)
                                    a.main.panning = SharpMikCommon.PAN_LEFT;
                                else if (a.main.panning > SharpMikCommon.PAN_RIGHT)
                                    a.main.panning = SharpMikCommon.PAN_RIGHT;
                            }
                        }
                    }

                    a.wantedperiod = a.tmpperiod = ModPlayer.GetPeriod(mod.flags, (ushort)(a.main.note << 1), a.speed);
                    a.main.keyoff = SharpMikCommon.KEY_KICK;
                }

                row.Cols.Add(col);
            }


            return row;
		}

	}



	public class Order
	{
		public List<Pattern> Tracks;

		public Order()
		{
			Tracks = new List<Pattern>();
		}
	}

	public class Pattern
	{
		public List<Row> Rows;

		public Pattern()
		{
			Rows = new List<Row>();
		}
	}

	public class Row
	{
		public List<Col> Cols;

		public Row()
		{
			Cols = new List<Col>();
		}

	}

	public class Col
	{
        public byte anote
        {
            set
            {
                switch (value)
                {
                    case 67: note = "F#6"; break;
                    case 66: note = "F-6"; break;
                    case 65: note = "E#6"; break;
                    case 64: note = "E-6"; break;
                    case 63: note = "D#6"; break;
                    case 62: note = "D-6"; break;
                    case 61: note = "C#6"; break;
                    case 60: note = "C-6"; break;
                    case 59: note = "F#5"; break;
                    case 58: note = "F#5"; break;
                    case 57: note = "F-5"; break;
                    case 56: note = "E#5"; break;
                    case 55: note = "E-5"; break;
                    case 54: note = "D#5"; break;
                    case 53: note = "D-5"; break;
                    case 52: note = "C#5"; break;
                    case 51: note = "C-5"; break;
                    case 50: note = "F#4"; break;
                    case 49: note = "F-4"; break;
                    case 48: note = "E#4"; break;
                    case 47: note = "E-4"; break;
                    case 46: note = "D#4"; break;
                    case 45: note = "D-4"; break;
                    case 44: note = "C#4"; break;
                    case 43: note = "C-4"; break;
                    case 42: note = "F#3"; break;
                    default: note = Convert.ToInt32(value).ToString(); break;
                }
            }
        }

        public int sample;
        public string name;
		public string note;
		public string effect;
		public int volume;
	}
}
