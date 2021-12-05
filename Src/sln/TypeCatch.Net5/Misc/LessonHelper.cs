using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using AAV.Sys.Helpers;

namespace TypingWpf
{
  public static class LessonHelper
  {
    static readonly Random _seed = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
    public const int PaddingLen = 250;

    static string GetLesson_Phrases(string lessonLenStr)
    {
      if (!int.TryParse(lessonLenStr, out var lessonLen))
        lessonLen = 100;

      if (lessonLen < 10)
      {
        lessonLen =
          lessonLen == 0 ? 10 :
          lessonLen == 1 ? 100 :
          lessonLen == 2 ? 200 :
          lessonLen == 3 ? 300 :
          lessonLen == 4 ? 400 :
          lessonLen == 5 ? 500 :
          100;
      }

      var random = _seed.Next(AllLines.Length - lessonLen - PaddingLen);
      var nextCr = AllLines.IndexOf("\r", random) + 2;
      if (nextCr + lessonLen + PaddingLen < AllLines.Length)
        random = nextCr;

      return AllLines.Substring(random, lessonLen + PaddingLen); // .Trim(); LessonLen = LessonText.Length;
    }
    public static void CodeGen()
    {
      var i = 0;
      foreach (var s in _specialDrill.Split(new[] { "[#]", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
      {
        if (s.Length > 77 || s.StartsWith("\t")) continue;
        var ln = $"<MenuItem Header=\"_{i + 1,-2:X} {s,-30}\"      Command=\"{{Binding LTCmd}}\" CommandParameter=\"D-{i++}\"  />";
        Trace.TraceInformation(ln);
      }
    }
    public static string GetLesson(LessonType _0to3, string sublesson)
    {
      string s;
      switch (_0to3)
      {
        case LessonType.BasicLessons: s = _basicLessons; break;
        case LessonType.Combinations: s = _combinations; break;
        case LessonType.DigitSymbols: s = _digitSymbols; break;
        case LessonType.SpecialDrill: s = _specialDrill; break;
        case LessonType.Experimental: s = getFromOneDrive(sublesson); return s;
        case LessonType.PhrasesRandm: s = GetLesson_Phrases(sublesson); return s;
        case LessonType.EditableFile: s = getCreateFromOneDrive(sublesson); return s;
        default: s = $"Nothing for {_0to3}"; break;
      }

      var ss = s.Split(new[] { "[#]" }, StringSplitOptions.RemoveEmptyEntries);

      if (int.TryParse(sublesson, out var sublessonInt))
        if (sublessonInt < ss.Length)
        {
          var sss = ss[sublessonInt].Split(new[] { '\n' });
          if (sss.Length > 1)
            return sss[1].Trim(new char[] { ' ', '\r', '\n', '\t' });
        }

      return "????????";
    }

    public static Dictionary<string, string> EditableFiles
    {
      get
      {
        if (_files == null)
        {
          _files = new Dictionary<string, string>();
          foreach (var file in Directory.GetFiles(OneDrive.Folder(_exercizeDir), "*.txt"))
          {
            _files.Add(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));
          }
        }
        return _files;
      }
    }
    static Dictionary<string, string> _files = null;


    static string getCreateFromOneDrive(string sublesson)
    {
      if (EditableFiles.ContainsKey(sublesson) && !string.IsNullOrWhiteSpace(EditableFiles[sublesson]))
        return EditableFiles[sublesson];
      else
      {
        var file = OneDrive.Folder($@"{_exercizeDir}{sublesson}.txt");
        if (!File.Exists(file))
        {
          var dflt =
              (sublesson.Equals("msa")) ? string.Format(_msa, string.IsNullOrEmpty(Environment.UserDomainName.ToLower()) ? "random" : Environment.UserDomainName.ToLower(), Environment.UserName.ToLower()) :
              (sublesson.Equals("nymi")) ? string.Format(_nym, Environment.UserName.ToLower()) :
              typeof(LessonHelper).GetField($"_{sublesson}", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null).ToString();

          using (FileStream fs = File.Create(file))
          {
            var info = new UTF8Encoding(true).GetBytes(dflt ?? $"Feel free to replace this with whatever excercise you deem fit...\r\n...preferably matching the topic of '{sublesson}'.");
            fs.Write(info, 0, info.Length);
            fs.Close();
          }

          var p = Process.Start("Notepad.exe", file);
          p.WaitForExit();
        }

        _files.Add(Path.GetFileNameWithoutExtension(file), File.ReadAllText(file));

        return EditableFiles[sublesson];
      }
    }
    static string getFromOneDrive(string sublessonId)
    {
      var file = OneDrive.Folder($@"{_exercizeDir}Excersize_{sublessonId}.txt");
      if (!File.Exists(file))
      {
        //Clipboard.SetText(file);
        File.Create(file).Close();
        var p = Process.Start("Notepad.exe", file);
        p.WaitForExit();
      }

      return File.ReadAllText(file);
    }

    public static string ExercizeDir => _exercizeDir;
    #region Excercises - hardcoded and defaults:
    const string
            _ght = @"ght ght ght ght ght ght ght ght ght ght 
night night night night night night night night night night 
might might might might might might might might might might 
midnight midnight midnight midnight midnight midnight midnight midnight midnight ",
            _msa = @"{0} {0} {0} {0} {0} {0} {0} {0} {0} {0} {1} {1} {1} {1} {1} {1} {1} {0}.{1} {0}.{1} {0}.{1} {0}.{1} @outlook.com @outlook.com @outlook.com @outlook.com {0}.{1}@outlook.com {0}.{1}@outlook.com {0}.{1}@outlook.com {0}.{1}@outlook.com",
            _nym = @"nymi nymi nymi nymi nymi nymi nymi nymi nymi nymi 
@nymi.com @nymi.com @nymi.com @nymi.com @nymi.com 
a{0}@nymi.com a{0}@nymi.com a{0}@nymi.com ",
            _the = @"the the the the the the the the the the 
them them them them them them them them them them 
then then then then then then then then then then 
there there there there there there there there there there ",
            _was = @"was was was was was was was was was was 
wase wase wase wase wase wase wase wase wase wase 
wased wased wased wased wased wased wased wased wased wased 
wax wax wax wax wax wax wax wax wax wax 
waxe waxe waxe waxe waxe waxe waxe waxe waxe waxe 
waxed waxed waxed waxed waxed waxed waxed waxed waxed waxed ",
            _whe = @"what what what what what what what what what what 
when when when when when when when when when when 
where where where where where where where where where where 
why why why why why why why why why why 
was was was was was was was was was was ",
            _tion = @"tion tion tion tion tion tion tion tion tion tion 
fiction fiction fiction fiction fiction fiction fiction fiction fiction fiction 
mention mention mention mention mention mention mention mention mention mention 
tention tention tention tention tention tention tention tention tention tention 
sanction sanction sanction sanction sanction sanction sanction sanction sanction sanction ",
            _exercizeDir = @"Public\AppData\TypeCatch\Excersize\",
          _basicLessons = @"[#] df  jk
fffjjj ffjfj jffjj fjfjf jjfjf fj fjf fjffj jfjfj jf jfj dddjjj ddjdj jddjj djdjd jjdjd dj df jd djddj jdjdj dj dfd fd jd jfd jdf kkkddd kkdkd dkkdd kdkdk ddkdk kf jdk kj kdkkd dkdkd kj dk jdk fkjd ffkfk kffkk fkfkf kf jfk kkfkf fkffk kfkfk fk fdk fkj dfk ddfdf fddff kf dfd ffdfd jd fdd fkd jjkjk kjjkk jk jdk jkjkj kj fkj djdfk jfkkd jdjkf fjddk fkfdj kdjjf kfkjd dkffj dj fdd jk dfd jdk jd kf djfk

[#] as  l;
ssslll sslsl lssll slsls llsls sl ls lsl slssl lslsl lf kl lsd sk sf sl fls dll jl js aaalll aalal laall alala llala la lak all alaal lalal ad adj lda alf alfa sal flak ;;;aaa ;;a;a a;;aa ;a;a; aa;a; ala; dak; kaka; ;a;;a a;a;a add jaffa; da aka dad; ss;s; ;ss;; s;s;s ;;s;s fss; js; s;ss; ;s;s; ass; lass; aasas saass ask sla asasa ssasa jsa sad sas ll;l; ;ll;; fl; l;l;l ;;l;l fla; fld; alas; ls;;a lal;s slaa; s;sal ;alls ;s;la a;ssl als fall dak das jad; adsl; skald; alk flask; falls adds fad; lass asks lad; sad dad ass as a flak;

[#] vb  nm
bbbnnn bbnbn nbbnn bnbnb nnbnb nan bab blab lan ban jan bnbbn nbnbn abba anna jab nab bass bask bad fan lab fab dab nb dan bland blank and bank vvvnnn vvnvn nvvnn vnvnv nnvnv val van vnvvn nvnvn java vs vb vandal vas vj lava naval dvd mmmvvv mmvmv vmmvv mvmvm vvmvm mak malm mvmmv vmvmv am lam mass mamma small mask jam sms jvm bbmbm mbbmm bmbmb mmbmb amb lamb bmbbm mbmbm bam jamb mamba vvbvb bvvbb vbvbv bvds vnnmnm mnnmm man mmnmn manna nam vnvbm nbmmv nvnmb bnvvm bmbvn mvnnb mbmnv vmbbn band van kvass lambda dvd nan jams small bank vandal

[#] tg  yh
tttyyy ttyty yttyy tytyt yytyt my tad tytty ytyty talk fat yak sat mat nat yam yd nasty navy tasty may tatty lastly gggyyy ggygy yggyy gygyg yygyg baggy gyggy ygygy gad slang lag jaggy gadfly mangy gabby hhhggg hhghg ghhgg hghgh gghgh hash ghat hghhg ghghg has handy dash aghast tthth htthh ththt hhtht that bath thtth hthth thank myth ggtgt tggtt gat gtgtg ttgtg tang yyhyh hyyhh shy yhyhy hhyhy hymn gygth ythhg ygyht tyggh thtgy hgyyt hthyg ghtty gyttja ghastly shaggy laystall fatly tansy gym lay by taffy sly yank tat many thanks

[#] er  ui
rrruuu rruru urruu rurur uurur radar ugly rug rurru ururu rub rural aura drum jury burn hurry guru ruth eeeuuu eeueu ueeuu eueue uueue sun eye vugh bevel gun eueeu ueueu leu lues neutral hue due fuel rueful guest heugh iiieee iieie eiiee ieiei eeiei evil fire ieiie eieie tie lie die tried ivied field reign rriri irrii ririr iirir irk sir rirri iriri iris trim bird rile girl firm eerer reerr there erere rrere here nerve uuiui iuuii suit guide uiuiu iiuiu build ruins eueri uriie ueuir rueei rireu ieuur iriue eirru vigil duke merit germinative guerilla mute university gurgle true rule during survey remain renumber

[#] qw  op
wwwooo wwowo owwoo wowow oowow wow low now owe woe wowwo owowo row owner wood woke worm wolf world how work qqqooo qqoqo oqqoo qoqoq ooqoq qua iq qoqqo oqoqo equal quad quake quiet quit unique queen quay quod quoin quorum quota quotha pppqqq ppqpq qppqq pqpqp qqpqp equip up pqppq qpqpq pal pig pen pan pap pepper pepsi rap vip tape pair palp opaque wwpwp pwwpp wpwpw ppwpw paw pow wpwwp pwpwp wrap wipe weep whip wimp qqwqw wqqww sow qwqwq wwqwq query oopop poopp pop opopo ppopo opel prop qoqwp owppq oqopw woqqp wpwqo pqoow pwpoq qpwwo quid pro quo equator whore purple equivoke whole power opaque

[#] xc  , .
ccc,,, cc,c, ,cc,, c,c,c ,,c,c cd, cop, cow, calc, c,cc, ,c,c, cap, cup, cut, copy, cake, come, accept, cock, clock, cache, cackle, cluck, clinic, click cancel xxx,,, xx,x, ,xx,, x,x,x ,,x,x xxl, xml, mix, six, x,xx, ,x,x, fax, fox, box, vox, dux, excel, onyx, exile ...xxx ..x.x x..xx .x.x. xx.x. lex. cox. .x..x x.x.x xmas. xerox. oxy. extreme. exact, relax, cc.c. .cc.. c.c.c ..c.c .ca .ch .com c.cc. .c.c. ch. ck. civ. con. xxcxc cxxcc calyx xcxcx ccxcx coax coccyx ,,.,. .,,.. ex., ,.,., ..,., etc., x,xc. ,c..x ,x,.c c,xx. c.cx, .x,,c .c.,x x.cc, p.c., i.e., e.g., www.pix.com, a.m., unix.com, xxx.com

[#] z ! ? /
zzz/// zz/z/ /zz// z/z/z //z/z jazz zoo/zone z/zz/ /z/z/ zero zap/zombie mezzo lazy/hazy gaze zoom/gizmo, crazy zigzag/puzzle !!!/// !!/!/ /!!// !/!/! //!/! hello/hi! go! !/!!/ /!/!/ welcome! thanks! bazooka! squeeze! ???!!! ??!?! !??!! ?!?!? !!?!? how?! who? ?!??! !?!?! toxic!? why? azure? ok! zz?z? ?zz?? z?z?z ??z?z oz? fuzzy zebra? z?zz? ?z?z? zeal? zinc? !!z!z z!!zz zip! !z!z! zz!z! zing! //?/? ?//?? yes/no? /?/?/ ??/?/ zephyr/pizza? !/!z? /z??! /!/?z z/!!? z?z!/ ?!//z ?z?/! !?zz/ zenith/frozen waltz tzar/lizzie pkunzip.zip? new! all right! what? which? are you here? good luck/bye!

[#] Alphabetic test
fall jam dash kid ska lad alf bank navy valid man gal ham talk yak rutty urn elf is wolf oven quasi pet xxl cot zone abcdefghijklmnopqrstuvwxyz fife jojoba doddle kick suss label aka bob nine vulva memo guggle hah totality yellowy river unused eke ivied willow oboe quark prep xerox clack zigzag zyxwvutsrqponmlkjihgfedcba

[#] Short words
hay heir harp half hang hack hex high echo earn elan eddy edit obit oboe obey owl oak okie dokey down duty dux dixy doxy xmas xtal nix next note name nape nail snap spar step slow shot shit qua quay quad quod quit pack pray pure pig pixy push rush rue rock rose rich ring main mad male melt milk make zero zest zonk zoom zinc zing year yelp ywis your chip char cut chut coxy cow kale ken knap keep keck kiwi with wind wick woke wont void volt vole vote vita vox vug very jerk joke jack jab job jamb jeep girl glue gold grow good luck lake lid lie lex live love

[#] Long words
allowable absolute black bonus bicycle expressive eating fortune finish frequency fuzzy imperial include kilobyte obtain throw tutor cherry casino cinema cajole candle rubric rover rigor unbound unique vampire velvet vogue yearly zombie zippy quack quick quake queen query quorum quadruple xenon xerox xistor xiphoid xylogen xanthoxylone nixie nothing network neighbour window weight world winch whose white south sounding submarine jumbo justify journey government paradox penalty paving pepsi guano apes memento mori

[#] CAPITAL letters
Fred Fuji Jerry January Dick Doom Kate Ken Sony Stamina Lily Lady April Alf Nick NB Victor Val Bob Bill Yes YOB Tom Tim Henry Hamlet Grace Glen Uncle Unreal Romeo RIP Irene ICQ Eve Earth Mylene MIB Chris Charles Oval Office Walt Wales Peg PS Quake Queen Xmas XXL Zaire Zorro Felix John December Kit Sam Luke Alex November VIP Boston Yahoo Ted Helen Gates UFO Rose Ira Ed May Carol October Winamp Pol Quantum XML Zanzibar Antonio Vivaldi Nino Rota Bon Jovi Led Zeppelin Pink Floyd Genesis Vaya Con Dios Enya

[#] ' ""()
((()))(()())(())()()())()( a) b) c) (home) ()(() )()() (Time), (mix), (Drill), (go) '''))) '')') )'')) ')')' ))')' I'm, it's (Don't) ')'') )')') isn't Can't (Father's) I'll """"""''' """"'""' '""""'' ""Hi!"" ""I'm hungry"" ""'""'"" ''""'"" ""Won't"", ""hasn't"", ""It's no good"", ""Bye"" ((""("" ""(("""" (""(""( """"(""( (""Ball""), (""Tie"") (""(("" ""(""("" (""Cow""), (""Wake up""), (""Fine"") ''('( (''(( (hasn't) '('(' (('(' (Doesn't) ))"")"" ""))"""" (""Disk"") "")"") """")"") (""Rome"") ')'("" )(""""' )')""( ()''"" (""(') ""'))( ""("")' '""(() ""I'll be back"", (""Don't cry""), ""L'Instant X"".

[#] ; : -
;;;::: ;;:;: :;;:: ;:;:; ::;:; a:b; c:d; ;:;;: :;:;: City: Color: Password: ;) :) e; f; g; ;;;--- ;;-;- -;;-- ;-;-; --;-; so-so; back-up; ;-;;- -;-;- about-face; yo-yo; hara-kiri ;-) ::-:- -::-- :-:-: --:-: chin-chin :) :-::- -:-:- fie-fie: you-know-what :-) ;;:-; :;-:: -;--: ping-pong ;-:;: ;-:-; -:-;: month: June; ;:-:;- ;-:-;: :-;-:; Coca-cola; name: Alice; hobby: music; etc; case: break; continue; Bye-bye

[#] Sentences with '
I'm sorry I'm late.You'll see. What's she like? It's all right! I'll call back later.Don't hang up. You don't understand me.No, I haven't. I'm in a hurry! They'll take this. What's wrong? I'm tired. Don't come in. Peter's friend. O'Henry.He doesn't like it. I'm lost.These men's plans. Don't cry.The cat's tail. No, she hasn't.I can't be with you. Don't speak.It's no good. What's my age again? Let's go!

[#] Punctuation marks
It's hasn't; I'll, They're wasn't. couldn't. ""What's it?"", ""Brian's brain""; ""I'm sorry!"" :o)) ;) :o((acc. (""account""); It's (It is); ex. (""example""); :)) :-) ;-( and/or yes/no - OK/Cancel :-) http://x.y.z/files/ How are you? ""Good-bye!"" Month/Day/Year; ""I've been robbed!"" :-( Really?

[#] Examination
Don't hurry! Fifth/fief/Forfend, Joe? jar? Jojoba? Dad! dud! Doom! (Kick) (kink) (Kodak). So-so - suss - Sister, Level; label; Lola; Abandon: abaca: Anna: 'Viva', 'valve', 'Volvo'. ""Nanny"", ""noun"", ""Nine"". Bubble/barber/Bob. Yearly? yummy? You? That - totter - Tit - (High) (hash) (Hatch). Gag! giggle! Gong! Uruguay; usual; Usurp; River: rural: Roper: 'Icing' 'iris' 'IQ', ""Eye"" ""elite"" ""Eden"". Mam/mummy/Mem. Cicero? cocky? Clock? October - oppose - Oboe - (Wow) (withdraw) (Windows). Pappy! pup! Pipe! Quit; qual; Quake; Xerxes: xerox: Xmas: 'Zanzibar', ""zigzag"", 'Zoo'. If you didn't look at the keyboard while typing, you can try working in ""Phrases"" mode!Regards.

",
          _combinations = @"[#] Combinations 1
th the they then them there their that this wh why who whom when where what while ea eat east tear read head beat deal leaf ed bed med led red tired shed liked armed moved en pen den ten often listen given taken even frozen happen broken er her ever over river after enter under order biker joker ar car bar mar oral polar solar star or for nor author actor major motor tutor sailor al pal gal dual usual rural royal vital total ay say day way ray pay lay may ly sly fly ugly only rally reply apply badly ry try cry dry fry very weary carry gh high sigh weigh bough though ew new few pew blew view ow now how low row sow yellow

[#] Combinations 2
un un un un un un unit uncle under unable unlock ing ing ing ing ing ing ping wing bring thing doing going ght ght ght ght ght ght night right weight straight thought ish ish ish ish ish dish wish finish publish longish ist ist ist ist ist ist fist list exist consist ous ous ous ous ous ous nous famous nervous house age age age age age age page stage manage voyage Garbage ant ant ant ant ant ant rant distant giant variant ment ment ment ment ment ment moment ferment equipment tion tion tion tion tion tion action option edition mention condition citation ture ture ture ture ture ture future culture nature picture

[#] Parts of speech 1
the the the the they they they they they they there there there there there there at at at at that that that that that that what what what what what what is is is is his his his his his his this this this this this this it it it it in in in in to to to to into into into into into into an an an an can can can can can can and and and and and and as as as as was was was was was was has has has has has has had had had had had had have have have have have have be be be be been been been been been been up up up up on on on on upon upon upon upon upon upon upon if if if if of of of or or or or for for for for for for from from from from from from who who who who who why why why why why which which which which which which which which which which we we we we were were were were were were are are are are are are

[#] Parts of speech 2
isn't isn't isn't isn't aren't aren't aren't aren't can't can't can't can't wasn't wasn't wasn't wasn't weren't weren't weren't weren't haven't haven't haven't haven't hasn't hasn't hasn't hasn't hadn't hadn't hadn't hadn't won't won't won't won't don't don't don't don't didn't didn't didn't didn't This's This's This's This's They're They're They're They're You're You're You're You're It's It's It's It's I'm I'm I'm I'm I'll I'll I'll I'll What's What's What's What's

[#] Parts of speech SHORT
isn't isn't What's

",
          _digitSymbols = @"[#] 56  78
666777 66767 76677 67676 77676 67667 76767 555777 55757 75577 57575 77575 57557 75757 576 75 67 657 888555 88585 58855 85858 55858 85885 58585 685 758 78 66868 86688 68686 88686 68668 86868 868 586 7568 55656 65566 Effel 65 56565 66565 77878 87788 867 758 78787 88787 57568 76885 75786 67558 68657 85776 86875 58667

[#] 34  90
444999 44949 94499 49494 99494 74 59 49449 94949 964 849 333999 33939 93399 39393 99393 83 49 39339 93939 593 739 000333 00303 30033 03030 33030 40 93 03003 30303 360 830 44040 04400 40404 00404 Error 404 40440 04040 Apollo 440 33434 43344 374 km 39 m 34343 44343 99090 09900 940 kg 90909 00909 39340 94003 93904 49330 40439 03994 04093 30449

[#] 12  -=
222-- - 22 - 2 - -22-- 2 - 2 - 2--2 - 2 20 8 - 2 2 - 22 - -2 - 2 - 111-- - 11 - 1 - -11-- 1 - 1 - 1--1 - 1 01 1 - 9 1 - 11 - -1 - 1 - 1 - 2 - 3 === 111 == 1 = 1 1 == 11 = 1 = 1 = 11 = 1 = 1 - 1 = 0 2 - 1 = 1 = 1 == 1 1 = 1 = 1 1 byte = 8 bits 22 = 2 = = 22 == 2 = 2 = 2 == 2 = 2 52 - 81 = -29 2 = 22 = = 2 = 2 = 11212 21122 71 82 12121 22121-- =-= = --== -2 - 1 = -3 -=-= - ==-= -1 - 12 = -2 == 1 - 1 -= 2 2 - 11 = 2 = 21 - = 1--2 = 2 = -1 1 = 22 - 7 - 5 = 2 Y2K, Blink 182, Sonata Nr. 14, 911, U - 96.mp3

[#] Numbers 01-99
01 08 03 07 05 09 04 06 02 10 80 30 70 50 90 40 60 20 14 17 15 19 13 18 12 10 16 41 71 51 91 31 21 01 25 29 23 27 21 20 24 28 26 52 92 72 12 02 42 82 62 35 38 31 30 34 37 32 39 36 53 83 13 03 43 73 23 93 63 41 48 46 40 42 47 45 49 43 14 84 64 04 24 74 54 94 34 52 58 54 50 53 57 51 59 56 25 85 45 05 35 75 15 95 65 68 61 67 64 69 62 60 63 56 86 16 76 64 96 26 06 36 72 78 74 70 75 79 71 76 73 27 87 47 07 57 97 17 67 37 82 80 85 89 83 87 81 84 86 28 08 58 98 38 78 18 48 68 92 97 94 90 96 98 93 91 95 29 79 49 09 69 89 39 19 59

[#] % ^  & *
^^^&& & ^^ &^ &&^^&& ^ &^ &^ &&^ &^ a & b, c & d, 3 ^ 8, 9 ^ 7 ^ &^^ &&^ &^ &a & b ^ j ^ k & l, drag & drop %%%&& & %% &% &&%%&& % &% &% &&% &% 75 % 50 % 15 % 0 % % &%% &&% &% &10 % &80 % 5 % ***%%% **% *% % **%% *% *% * %% *% *5 * 8 = 40 4 * 8 *% **% % *% *% 5 * 39 % 28 * 2 85 * 8 % ^^ *^ **^^ ** ^ *^ *^ **^ *^ 3 * 2 ^ 7 ^ *^^ **^ *^ *8 ^ 2 * 6 %%^%^ ^%%^^ 30 % %^%^% ^^%^% 7 ^ 2 && *&**&& **3 * 4 & *&*&**&*&cash & carry % &%^ *&^ **% &% &*^ ^ &%% * ^ *^% &*%&&^ *^ *&% % *^^ &Erase & Rewind

[#] £ $  ( )
$$$((( $$($(($$(( $($($ (($($ $84 $0 $($$(($($( $74 $2.94 $8.49 £££((( ££(£((££(( £(£(£ ((£(£ £96 £43 £75 £(££((£(£( £93 £13 £837 ))£)£ £))££ )£)£) ££)£) (£9) (£0.3) )£))£ £)£)£ (£9.3) (£3) (£0.1) $$)$) )$$)) $)$)$ ))$)$ ($04) ($94) $)$$) )$)$) ($74) ($8) ££$£$ $££$$ £73 £$£$£ $$£$£ $04 (()() )(()) (Yes) ()()( ))()( (A) (B) £(£$) ($))£ (£()$ $(££) $)$£( )£(($ )$)(£ £)$$(

[#] ! @  _ +
@@@___ @@_@_ _@@__ @_@_@ __@_@ my_mail@domen.com @_@@_ _@_@_ a_b@aol.com !!!___ !!_!_ _!!__ !_!_! __!_! Hi! I_love_you! !_!!_ _!_!_ +++!!! ++!+! !++!! +!+!+ !!+!+ 1+1=2! 2+2=4! +!++! !+!+! 3+2=5! 5+2=7! @@+@+ +@@++ @+@+@ ++@+@ 47+38 @+@@+ +@+@+ info@mail.com !!@!@ @!!@@ Wow! !@!@! @@!@! @:-) ;@) __+_+ +__++ _accent_ _+_+_ ++_+_ 4+3 !_!@+ _@++! _!_+@ @_!!+ @+@!_ +!__@ +@+_! !+@@_ b_g@microsoft.com!

[#] ` ¬  # ~
```### ``#`# #``## `#`#` ##`#` `#``# #`#`# ¬¬¬### ¬¬#¬# #¬¬## ¬#¬#¬ ##¬#¬ ¬#¬¬# #¬#¬# ~~~¬¬¬ ~~¬~¬ ¬~~¬¬ ~¬~¬~ ¬¬~¬~ ~¬~~¬ ¬~¬~¬ ``~`~ ~``~~ `~`~` ~~`~` `~``~ ~`~`~ ¬¬`¬` `¬¬`` ¬`¬`¬ ``¬`¬ ##~#~ ~##~~ #~#~# ~~#~# ¬#¬`~ #`~~¬ #¬#~` `#¬¬~ `~`¬# ~¬##` ~`~#¬ ¬~``#

[#] [ ] { }
		[[[]]] [[]
		[] ][[]] []
		[]
		[ ]][]
		[ [a, b]
		[c, d]
		[]
		[[] ][]
		[]
		[3;8] {{{]]] {{]{] ]{{]] {]{]{ ]]{]{ {:-] {]{{] ]{]{] {;-] }}}{{{ }}{}{ {}}{{ {5;4} }{}{} {{}{} {6;2} }{}}{ {}{}{ {3,4} {48,91} [[}
		[} }[[}} [}[}[ }}[}[ [}[[} }[}[} [1,2} {{[{[ [{{[[ {[{[{ [[{[{ {3,4] ]]}]} }]]}} ]}]}] }}]}] {]{[} ][}}{ ]{]}[ []{{} [}[{] }{]][ }[}]{ {}[[]
		H1 { color:red; }

[#] \ | < >
\\\<<< \\<\< <\\<< \<\<\ <<\<\ C:\Temp\ 1<8 \<\\< <\<\< C:\Progra~1\ 2-1<4 |||<<< ||<|< <||<< |<|<| <<|<| M|N |<||< <|<|< :-| ;| >>>||| >>|>| |>>|| >|>|> ||>|> <a, b> >|>>| |>|>| <c|d> 1<2, 5>4 \\>\> >\\>> \>\>\ >>\>\ >:-\ \>\\> >\>\> ||\|\ |\|\| \\|\| <<><> <><>< >><>< |<|\> <\>>| <|<>\ \<||> \>\|< >|<<\ >\><| |>\\< >x><z \ z>xX< | ><>., \ ,><<.

[#] All symbols
3£3£3 ~£~£~£ £3 £23 £8.3 )0)0) :-0 ;) 2@2@2 {@{@{@ user@hotmail.com; 9(9(9 :-9 ;-( 4$4$4 _$_$_$ $4 $1.40 $4.99 7&7&7 &^&^&^ wash & go, black & white 6^6^6 2^8 9^6; 5%5%5 }%}%}% 50% 25% 0%; 8*8*8 *<*<*> *.mp3 6*8=48 1!1!1 !>!>!< OK! Hi! How do you do? 4/2=2 8/2=4 3+2=5 1+3=4 1+2+3+4+5=15 Are you ready? Really? [`[`[` ]¬]¬]¬ #£#£#£ _@_@_%_% login@aol.com; `}`}`} £*£*_@_~ #¬#¬{¬{ [^[^]^]^ _£_$_%#@ [@_~`#* 5-!=? 9+3-4 _3(3)_ {%&£&@} $]$¬_ @&$&*~ a>=b>=c ¬$#[£&` _@_¬#` 1<3<7 5/3 7/4 9/2 %`&¬~¬ ^@_¬}` &$¬[~` #`[¬*@ F(x)<=F[y]>=F{z}

",
          _specialDrill = @"[#] Smiles :-)
:) :-) ;-) :=) ;=) :o) ;o) :-/ ;-\ B=) B-) 8-) :-P :-b :=D :-o <:-) ;^) &:-) @;-) %-) >:-( :-@ 8:-) :-{) :-{} {:-) :-7 :-9 :=] ;( :-( ;-0 :-| :( ;-( :o( ;o( ;=( B-| B-( 8=) :-)))))

[#] HTML
/#/#/# #'#'#' %""%""%"" %&%&%& _/_/_/ ;@;@;@ -:-:-: <!<!<! >=>=>= =""#993366"" ='30%' <a href=mailto:x@y.z> <html> <head><title>Welcome</title></head> <body bgcolor=#0F0F0F leftmargin=0><p align=""right"">Hi, people!</p> <table border=""0"" width=76%><tr><td>&nbsp;</td></tr></table> <div align='center' width='50%'><p style=""border:1px solid #339966"">&lt; &nbsp; &quot; &amp; &gt; </p></div> <!-- comments --> <hr width=82% color=""#38f7e9""><a href=""http://staminapro.com"" target=_blank>Home</a></body></html>

[#] NumPad (digits only)
5466¶4560¶45405¶5606¶5405¶6504¶4605¶46460¶5880¶580486¶80688¶4522¶5224¶02286¶4022¶8546¶0822¶8508¶26022¶8077¶508787¶04776¶0885¶7027¶70775¶6075¶7467¶2095¶9959¶09699¶4902¶929299¶80658¶9980¶4994¶90225¶8906¶77290¶115¶15102¶1151106¶1411¶480¶9119¶11076¶15810¶491¶851¶06842¶06789¶10461¶210¶115¶811¶7033¶5334¶603¶351¶30¶3642¶033¶5833¶031¶633031¶3130348¶609¶941387¶0564¶023203¶4602¶790¶3180¶513076¶704¶6805050¶1000¶50807¶6109¶00080¶88192¶2001¶2002¶1978¶1700¶6810¶4058¶6506¶06505¶0030¶2010¶1834¶5123¶0234¶07764¶8127¶703577¶6276¶7672¶712¶97437

[#] NumPad
7/8/9¶5/6/4¶3/1/2¶9/5/1¶87/46/32¶8*7*9¶5*6*4¶2*1*3¶7*5*3¶89*46*12¶7-9-8¶5-6-4¶3-1-2¶8-4-3¶79-65-20¶9+7+8¶5+4+6¶2+1+3¶7+6+2¶98+54+30¶7.98¶5.64¶3.12¶0.951¶3.14¶7/5*3-1+3.08¶76+84-943+80+215-30-51¶286/14*9*372/84*50/306¶.315*.486+1.509-.75/6.02¶45+5.18+862-.75-13/.94/26*3.14*80+1.27*50-6.52/9

";

    public enum LessonType
    {
      BasicLessons,
      Combinations,
      DigitSymbols,
      SpecialDrill,
      PhrasesRandm,
      Experimental,
      EditableFile
    }
    #endregion

    #region Core texts:
    const string AllLines = @"
You can't shine like a diamond, if you not willing to get cut like a diamond!
A smile is an inexpensive way to change your looks.
It is a common delusion that you can make things better by talking about them.
Many people lose their tempers merely from seeing you keep yours.
The truth hurts, and so would you if you were stretched as much.
Being rich is having money; being wealthy is having time.
I've learned that even when I have pains, I don't have to be one.
I am thankful for laughter, except when milk comes out of my nose.
A good laugh and a long sleep are the best cures in the doctor's book.
Life is like a ten-speed bike. Most of us have gears we never use.
If you are the smartest person in the room, then you are in the wrong room.
A smile is a curve that sets everything straight.
Everything is changing. People are taking their comedians seriously and the politicians as a joke.
A business like an automobile, has to be driven, in order to get results.
Nothing is impossible, the word itself says I'm possible!
Everything is funny, as long as it's happening to somebody else.
I will not let anyone walk through my mind with their dirty feet.
You cannot be anything if you want to be everything.
You can't cross the sea merely by standing and staring at the water.
Talkers are usually more articulate than doers, since talk is their specialty.
I was brought up to respect my elders, so now I don't have to respect anybody.
It is sad to grow old but nice to ripen.
The truth can be funny but it's not funny to cover up the truth.
A bank is a place that will lend you money if you can prove that you don't need it.
A laugh is a smile that bursts.
They are ill discoverers that think there is no land when they see nothing but sea.
The best lightning rod for your protection is your own spine.
The Lord prefers common-looking people. That is why he made so many of them.
Cause your facial expression to change - smile.
We don't stop playing because we grow old; we grow old because we stop playing.
Actually being funny is mostly telling the truth about things.
Today's opportunities erase yesterday's failures.
Money won't buy happiness, but it will pay the salaries of a large research staff to study the problem.
A man who correctly guesses a woman's age may be smart, but he's not very bright.
One fails forward toward success.
If you dig a grave for others you may fall into it yourself.
Middle age is when you can still do everything you used to do - but you decide you'll do it tomorrow.
A study of economics usually reveals that the best time to buy anything is last year.
Some people die at twenty-five and aren't buried until they are seventy-five.
Men are like wine. Some turn to vinegar, but the best improve with age.
The shortest recorded period of time lies between the minute you put some money away for a rainy day and the unexpected arrival of rain.
If you reach for a star, you might not get one. But you won't come up with a hand full of mud either.
Keep your sense of humor. As General Joe Stillwell said, 'The higher a monkey climbs, the more you see of his behind'.
For a politician to complain about the press is like a ship's captain complaining about the sea.
If you are always trying to be normal, you will never know how amazing you can be.
See the world like a big wardrobe. Everybody has his own costume. There is only one that fits you perfectly.
I don't think God put you on this earth just to make millions of dollars and ignore everything else.
You can't experience simple joys when you're living life with your hair on fire.
The mind is like a clock that is constantly running down. It has to be wound up daily with good thoughts.
If your capacity to acquire has outstripped your capacity to enjoy, you are on the way to the scrap-heap.
From his neck down a man is worth a couple of dollars a day, from his neck up he is worth anything that his brain can produce.
Fire and swords are slow engines of destruction, compared to the tongue of a gossip.
Women are like teabags. We don't know our true strength until we are in hot water!
My father taught me a good lesson: Don't get to low when things go wrong. And don't get too high when things are good.
New Year's resolutions work like this: you think of something you enjoy doing and then resolve to stop doing it.
If you think you are too small to be effective, you have never been in bed with a mosquito.
Common sense and a sense of humor are the same thing, moving at different speeds. A sense of humor is just common sense, dancing.
Everyone has an invisible sign hanging from his neck saying, Make Me Feel Important! Never forget this when working with people.
Yoga is the fountain of youth. You're only as young as your spine is flexible.
Too bad that all the people who know how to run the country are busy driving taxicabs and cutting hair.
You need to be comfortable with you. Stop worrying about the couple in the corner who may or may not be looking at you funny.
Tension is a habit. Relaxing is a habit. Bad habits can be broken, good habits formed.
How can they say my life is not a success? Have I not for more than sixty years gotten enough to eat and escaped being eaten?
Chasing success is like trying to squeeze a handful of water. The tighter you squeeze, the less water you get. 
When you chase it, your life becomes the chase, and you become a victim of always wanting more.
Going to church doesn't make you a Christian any more than going to a garage makes you an automobile.
Isn't it interesting that the same people who laugh at science fiction listen to weather forecasts and economists?
People never lie so much as after a hunt, during a war or before an election.
In every dream journey there comes a moment when you have to quit living as if the purpose of life is to arrive safely at death.
You can clutch the past so tightly to your chest that it leaves your arms to full to embrace the present.
People who are wrapped up in themselves make small packages.
People often say that motivation doesn't last. Well, neither does bathing - that's why we recommend it daily.
You cannot soar with the eagles as long as you hang out with the turkeys.
When you come to the end of your rope, tie a knot and hang on.
I should be a postage stamp. That's the only way I'll ever get licked!
You may not realize it when it happens, but a kick in the teeth may be the best thing in the world for you.
Earth provides enough to satisfy every man's needs, but not every man's greed.
You want to be a millionaire. But your values system says you believe in sleep more than grinding!
Actions Speak Louder Than Words.
Spring is the time of year when it is summer in the sun and winter in the shade.
You can practice shooting 8 hours a day, but if your technique is wrong, then all you become is very good at shooting the wrong way.
I don't want to be a genius - I have enough problems just trying to be a man.
Nothing in the world is ever completely wrong. Even a stopped clock is right twice a day.
Wrinkles should merely indicate where smiles have been.
Be nice to nerds. Chances are you'll end up working for one.
You can tell a lot about a person by the way they handle three things: a rainy day, lost luggage and tangled Christmas tree lights.
I ask people if an elephant has ever bitten them. Most of the time people say no. But everyone has been bitten by a mosquito. 
It's the little things that get us.
Money is like manure, of very little use except it be spread.
Enthusiasm is the leaping lightning, not to be measured by the horse-power of the understanding.
Asking is the beginning of receiving. Make sure you don't go to the ocean with a teaspoon. At least take a bucket so the kids won't laugh at you.
An appeaser is one who feeds a crocodile, hoping it will eat him last.
Aerodynamically the bumblebee shouldn't be able to fly, but the bumblebee doesn't know that so it goes on flying anyway.
No one has ever become poor by giving.
I am not funny. My writers were funny. My direction was funny. The situations were funny. But I am not funny. I am not funny. What I am is brave.
Do not hold to what you have. It is like a ferry boat for people who want to get across waters. 
Once you have got across, never bear it on your back. You should head forward.
Life is the movie you see through your own eyes. It makes little difference what's happening out there. It's how you take it that counts.
Here is the test to find whether your mission on earth is finished. If you're alive, it isn't.
Remember that the most beautiful things in the world are the most useless: peacocks and lilies for instance.
We humans are a silly bunch. We spend half our time trying to fit in with the crowd and the other half trying to stand out from it.
Can I give you a handful of laughter, a smidgen of giggles to boot, a cupful of tease and a comical sneeze, followed by a hilarious hoot. John McLeod,
Don't complain about the snow on your neighbour's roof when your own doorstep is unclean.
To die laughing must be the most glorious of all glorious deaths!
He knows nothing; he thinks he knows everything - that clearly points to a political career.
If a cluttered desk is a sign of a cluttered mind, of what, then, is an empty desk a sign?
Did you ever notice how difficult it is to argue with someone who is not obsessed with being right?
Years ago there was belief that the world was flat. 
People were born into that belief and they took it on faith that if they went too far from the shoreline in a boat they would fall off the earth. 
Columbus sailed on.
Happen to things, don't let things happen to you.
Life is a hurdle race, the winner has to cross all the hurdles and still maintain enthusiasm.
Give me six hours to chop down a tree and I will spend the first four sharpening the axe.
I don't want problems solved for me. I want the fishing rod, not the fish.
A bore is a person who opens his mouth and puts his feats in it.
It's always too early to quit.
Man does not control his own fate. The women in his life do that for him.
Lower your expectations of earth. This isn't heaven, so don't expect it to be.
Give me a stock clerk with a goal, and I will give you someone who will make history. Give me someone without a goal, and I will give you a stock clerk.
Sports are the reason I am out of shape. I watch them all on TV.
Interest without activity is similar to having a vehicle that is out of gas - it won't take you anyplace.
Without the will or desire to achieve one is like flotsam on the oceans of time.
The reason why people give up so fast is because they tend to look at how far they still have to go, instead of how far they have come.
The fellow who never makes a mistake takes his orders from one who does.
Three groups spend other people's money: children, thieves, politicians. All three need supervision.
Quality is more than important than quantity. One home run is much better than two doubles.
Why hoard your troubles? They have no market value, so just throw them away.
Procrastination is an art form that is less desirable than painting a detailed landscape using a three inch wide brush.
Without a plan of action to put how a dream is envisioned to play out is like pouring out of picture of water on the ground and expecting it to stay in on the surface of the ground.
Negative people aren't concentrating their bad energy to be shared with only you. 
Mosquitos aren't going to bite just you; they are going to go after the easiest available blood.
Don't sweat the petty things and don't pet the sweaty things.
The really frightening thing about middle age is the knowledge that you'll grow out of it.
I was thinking about how people seem to read the Bible a whole lot more as they get older; then it dawned on me - they're cramming for their final exam.
Turning our abilities from stagnate puddles to rushing rivers can happen when we apply ourselves to the task.
Our minds are like monkeys, swinging from one thought to another like monkeys on a tree. 
As a result, we always feel as if a sense of order, balance, awareness and concentration elude us, because we are always doing, always acting.
An economist is an expert who will know tomorrow why the things he predicted yesterday didn't happen today.
Remember that sometimes people laugh when something is actually funny, but often they laugh when they lack the imagination to understand the situation.
One of the most successful ways companies get the consumer's attention is by presenting the product in a funny or emotional way.
Establishing goals is all right if you don't let them deprive you of interesting detours.
Build on what makes you different from your competition... You need to be the red tree in the forest.
To Retirement! It's nice to get out of the rat race, But you have to learn to get along with less cheese.
You only need listen to yourselves. It reminds me of an old proverb: 'If and When were planted, and Nothing grew'.
It is difficult, but not impossible, to conduct strictly honest business.
The trouble with not having a goal is that you can spend your life running up and down the field and never score.
If you had to identify, in one word, the reason why the human race has not achieved, and never will achieve, its full potential, that word would be meetings.
A funny thing happens when we start keeping promises to ourselves - we become unstoppable.
Visualize something totally funny or crazy! This will instantly change how you feel because you can't visualize two things at the same time.
I am not an advocate of pity parties. They allow you to wallow in misery and drag yourself even further into the depths of woe. 
But sometimes having a good cry is like changing the oil in your car. 
You got 3000 miles out of that batch, now you have 3000 miles to go before another.
To be taught to read - what is the use of that, if you know not whether what you read is false or true?
If everything is coming your way, then you're in the wrong lane.
A lie travels round the world while truth is putting her boots on.
Growing old is mandatory. Growing up? Definitely optional.
Wise men don't need advice. Fools won't take it.
If you are willing to admit faults, you have one less fault to admit.
Learn from the mistakes of others. You can't live long enough to make them all yourself.
If you think education is expensive, try ignorance.
There is nothing permanent except change.
When you're thirsty it's too late to think about digging a well.
Experience is that marvelous thing that enables you to recognize a mistake when you make it again.
Romantic love is a lightning bolt. Married love is an electric current.
Choose your wife as you wish your children to be.
A good beginning makes a good end.
At the touch of love, everyone becomes a poet.
There is no difference between a wise man and a fool when they fall in love.
The woman cries before the wedding and the man after.
Marriages are not as they are made, but as they turn out.
Marriage is a three ring circus: engagement ring, wedding ring, and suffering.
One good husband is worth two good wives; for the scarcer things are, the more they are valued.
Men marry women with the hope they will never change. Women marry men with the hope they will change. Invariably they are both disappointed.
Out of the frying pan and into the fire.
Don't count your chickens before they're hatched .
To quarrel with a drunk is to wrong a man who is not even there.
If glory comes after death, I'm not in a hurry.
A closed mouth catches no flies.
A jack of all trades is master of none.
Don't bite off more than you can chew.
There's no arguing with the barrel of a gun.
He who laughs last thinks slowest!
A peacock who sits on his tail is just another turkey.
If you want to go fast, go alone. If you want to go far, go together.
Fall seven times, stand up eight.
Shared joy is a double joy; shared sorrow is half a sorrow.
Words should be weighed, not counted.
If you can't live longer, live deeper.
Do good and throw it in the sea.
Where love reigns, the impossible may be attained.
It's better to light a candle than curse the darkness.
A man who uses force is afraid of reasoning.
Still waters run deep.
He who does not travel, does not know the value of men.
The night rinses what the day has soaped.
Measure a thousand times and cut once.
A spoon does not know the taste of soup, nor a learned fool the taste of wisdom.
The most beautiful fig may contain a worm.
Change yourself and fortune will change.
In love, there is always one who kisses and one who offers the cheek.
Evil enters like a needle and spreads like an oak tree.
Who begins too much accomplishes little.
Whoever gossips to you will gossip about you.
Beauty lies in the eye of the beholder.
Don't sail out farther than you can row back.
There is no shame in not knowing; the shame lies in not finding out.
Age is honorable and youth is noble.
In a battle between elephants, the ants get squashed.
If you take big paces, you leave big spaces.
Before you score, you first must have a goal.
Good advice is often annoying, bad advice never is.
Give a man a fish, and you feed him for a day. Teach a man to fish, and you feed him for a lifetime.
Do not rejoice at my grief, for when mine is old, yours will be new.
What you see in yourself is what you see in the world.
It takes a whole village to raise a child.
Examine what is said, not who speaks.
Two wrongs don't make a right.
A large chair does not make a king.
Instruction in youth is like engraving in stone.
Deep doubts, deep wisdom; small doubts, small wisdom.
A man does not seek his luck; luck seeks its man.
A teacher is better than two books.
A beautiful thing is never perfect.
The reputation of a thousand years may be determined by the conduct of one hour.
Character is always corrupted by prosperity.
A fault confessed is half redressed.
To be willing is only half the task.
Everyone is kneaded out of the same dough, but not baked in the same oven.
The heart that loves is always young.
He who always thinks it is too soon is sure to come too late.
Turn your face to the sun and the shadows will fall behind you.
When the sun rises, it rises for everyone.
No man can paddle two canoes at the same time.
If you go to a donkey's house, don't talk about ears.
Speak the truth, but leave immediately after.
A successful marriage isn't finding the right person, it's being the right person.
If the grass is greener on the other side of the fence, you can bet the water bill is much higher.
It isn't difficult to make a mountain out of a molehill - just add a little more dirt.
Kind and loving words are windows to the heart.
People who feel they need control lack self-control.
Standing in the middle of the road is dangerous, you will get knocked down by the traffic from both directions.
The best way to get even is to forgive and then forget!
The mighty oak tree was once a little nut that held its ground.
The tongue must be heavy indeed, so few people can hold it.
You'll notice that a turtle only makes progress when it sticks out its neck.
It doesn't matter if you're on the right track, if you don't move, you'll get run over.
My husband forgot the code to turn off the alarm.
When the police came, he wouldn't admit he'd forgotten the code.
Some people say that I must be a horrible person, but that's not true.
If you water it and it dies, it's a plant.
If you pull it out and it grows back, it's a weed.
Clothes make the man.
Naked people have little or no influence on society.
The only winner in the War of 1812 was Tchaikovsky.
Blessed are we who can laugh at ourselves for we shall never cease to be amused.
Learn from the past.
Look to the future.
Live in the present.
Life is ours to be spent, not saved.
What makes old age so sad is not that our joys but our hopes cease.
If you wish you be like someone else, you waste the person you are.
Nothing can bring you peace but yourself.
The surest way of severely upsetting yourself for hours is by continuing to consider what concerns you most for a single moment too long.
Anxiety is interest paid on trouble before it is due.
Even the most tempting rose has thorns.
A home is not a mere transient shelter: its essence lies in the personalities of the people who live in it.
A lifetime of happiness!
I advise you to go on living solely to enrage those who are paying your annuities.
It is the only pleasure I have left.
Some of us just tend to look up at the stars.
All who would win joy, must share it; happiness was born a twin.
Don't underestimate the value of doing nothing, of just going along, listening to all the things you can't hear, and not bothering.
For those who fight for it, life has a flavour the sheltered will never know.
Live each day as if it were the last day of your life, because so far, it is.
To live a perfect life, you must ask nothing, give nothing, and expect nothing.
Expect everything, and anything seems nothing.
Expect nothing, and anything seems everything.
If we couldn't laugh, we'd all go insane.
If life doesn't offer a game worth playing, then invent a new one.
There is more to life than increasing its speed.
Freedom is the freedom to say that two plus two equals four.
If that is granted, all else follows.
Anxiety is a thin stream of fear trickling through the mind.
If encouraged, it cuts a channel into which all other thoughts are drained.
There is no excellent beauty that hath not some strangeness in the proportion.
Comedy is tragedy plus time.
Though we travel the world over to find the beautiful, we must carry it with us or we find it not.
He is happiest who hath power to gather wisdom from a flower.
Do not scorn the person who is perpetually happy.
He does know something you don't.
Joy is not in things, it is in us.
So go for the jump, and chase all your dreams.
It is not in the stars to hold our destiny, but in ourselves.
I love it.
I never want to die.
Everyone smiles in the same language.
So of cheerfulness, or a good temper, the more it is spent, the more it remains.
Life is short.
Live it up.
Every heart that has beat strong and cheerfully has left a hopeful impulse behind it in the world, and bettered the tradition of mankind.
Laughter is a tranquilizer with no side effects.
Living well is the best revenge.
Happiness is good health and a bad memory.
Happiness isn't something you experience; it's something you remember.
Here's to your love, health, and wealth - and time to enjoy each.
Waste not fresh tears over old griefs.
Learn not only to find what you like, learn to like what you find.
Realize that if you have time to whine and complain about something then you have the time to do something about it.
Ask yourself whether you are happy, and you will cease to be so.
I'm an idealist.
I don't know where I'm going, but I'm on my way.
The human race has one really effective weapon, and that is laughter.
I can't complain, but sometimes I still do.
Nostalgia is the realization that things weren't as unbearable as they seemed at the time.
Harmony seldom makes a headline.
And when it rains on your parade, look up rather than down.
Without the rain, there would be no rainbow.
There's a difference between beauty and charm.
The more I know men, the more I love my dog.
Everything else is just details.
The only thing we learn from history is that we learn nothing from history.
Prophecy is many times the principal cause of the events foretold.
This sentence contradicts itself - no actually it doesn't.
Beware of the man who won't be bothered with details.
That which is static and repetitive is boring.
That which is dynamic and random is confusing.
In between lies art.
Arithmetic is being able to count up to twenty without taking off your shoes.
Life is a whim of several billion cells to be you for a while.
A budget is just a method of worrying before you spend money, as well as afterward.
Not all chemicals are bad.
For example, without chemicals such as hydrogen and oxygen, there would be no way to make water, a vital ingredient in beer.
Teenaged girls use make-up to feel older sooner.
Their mothers use make-up to feel younger longer.
A mother may hope that her daughter will get a better husband than she did but she knows her son will never get as good a wife as his father did.
It is the mark of an educated mind to rest satisfied with the degree of precision which the nature of the subject admits and not to seek exactness where only an approximation is possible.
I do not fear computers.
I fear the lack of them.
Every sentence that I utter must be understood not as an affirmation, but as a question.
The true thinker walks freely.
My sources are unreliable, but their information is fascinating.
The economy depends about as much on economists as the weather does on weather forecasters.
The so-called lessons of history are for the most part the rationalizations of the victors.
History is written by the survivors.
Interestingly, according to modern astronomers, space is finite.
This is a very comforting thought - particularly for people who can never remember where they have left things.
It is not necessary to understand things in order to argue about them.
The opposite of a profound truth may well be another profound truth.
The most likely way for the world to be destroyed, most experts agree, is by accident.
That's where we come in; we're computer professionals.
We cause accidents.
Your theory is crazy, but it's not crazy enough to be true.
Math is like love - a simple idea but it can get complicated.
Anyone who goes to a psychiatrist ought to have his head examined.
Technological progress has merely provided us with more efficient means for going backwards.
The truth is out there.
Reality is that which, when you stop believing in it, doesn't go away.
The danger today is not so much that machines will learn to think and feel but that men will cease to do so.
Strong words are required for weak principles.
They would ask him to dinner, hear what he had to say, and make fun of it.
Logic is a system whereby one may go wrong with confidence.
Organic chemistry is the chemistry of carbon compounds.
Biochemistry is the study of carbon compounds that crawl.
Give me a lever long enough, and a prop strong enough, and I can singlehandedly move the world.
The heresies we should fear are those which can be confused with orthodoxy.
According to the latest official figures, 43% of all statistics are totally worthless.
As far as the laws of mathematics refer to reality, they are not certain, and as far as they are certain, they do not refer to reality.
Imagination is more important than knowledge.
Never worry about theory as long as the machinery does what it's supposed to do.
There's always an easy solution to every human problem - neat, plausible, and wrong.
There is no conversation more boring than the one where everybody agrees.
Not everything that can be counted counts, and not everything that counts can be counted.
Wonder, rather than doubt, is the root of knowledge.
It is impossible to travel faster than the speed of light, and certainly not desirable, as one's hat keeps blowing off.
Logic is like the sword - those who appeal to it shall perish by it.
The most incomprehensible thing about the world is that it is comprehensible.
In prayer, it is better to have a heart without words than words without heart.
It is bad luck to be superstitious.
History is the version of past events that people have decided to agree upon.
No great advance has ever been made in science, politics, or religion, without controversy.
It is always easier to believe than to deny.
Our minds are naturally affirmative.
Any sufficiently advanced technology is indistinguishable from magic.
Absence of proof is not proof of absence.
I think, therefore I am.
Put your hand on a hot stove for a minute, and it seems like an hour.
The biggest difference between time and space is that you can't reuse time.
The hands that help are better far than the lips that pray.
Whenever anyone says anything he is indulging in theories.
Ignorance is the mother of devotion.
The most beautiful thing we can experience is the mysterious.
It is the source of all true art and science.
No idea is so antiquated that it was not once modern; no idea is so modern that it will not someday be antiquated.
To ""be"" means to be related.
Religion is the opium of the masses.
Beware the man of one book.
It may be that our role on this planet is not to worship God but to create him.
There are three kinds of lies: Lies, Damn Lies, and Statistics.
For a successful technology, reality must take precedence over public relations, for Nature cannot be fooled.
The test of a first-rate intelligence is the ability to hold two opposed ideas in the mind at the same time, and still retain the ability to function.
Truth decays into beauty, while beauty soon becomes merely charm.
Charm ends up as strangeness, and even that doesn't last, but up and down are forever.
It took me fifteen years to discover that I had no talent for writing, but I couldn't give up because by that time I was too famous.
All progress is based upon a universal innate desire on the part of every organism to live beyond its income.
The only way to discover the limits of the possible is to go beyond them into the impossible.
If at first you don't succeed, failure may be your style.
Become addicted to constant and neverending self-improvement.
There's nothing so useless as doing efficiently that which should not be done at all.
If you're strong enough, there are no precedents.
Seventy percent of success in life is showing up.
If at first you don't succeed, redefine success.
New York City: No matter how many times I visit this great city I'm always struck by the same thing: a yellow taxi cab.
There are no depths to which I will not sink.
It looks like blind screaming hedonism won out.
You can run with the big dogs or sit on the porch and bark.
Hatred: A sentiment appropriate to the occasion of another's superiority.
Brooks' Law: Adding manpower to a late software project makes it later.
If you pray for rain, don't be surprised if you're struck by lightning.
Focus 90% of your time on solutions and only 10% of your time on problems.
Attempt the impossible in order to improve your work.
It's kind a fun to do the impossible.
It is better to be defeated on principle than to win on lies.
We must believe in luck.
For how else can we explain the success of those we don't like?
Anything is possible if you wish hard enough.
Freedom is nothing else but a chance to be better.
Don't fear change - embrace it.
To gain that which is worth having, it may be necessary to lose everything else.
You've got to be very careful if you don't know where you're going, because you might not get there.
Happiness: An agreeable sensation arising from contemplating the misery of another.
This is as true in everyday life as it is in battle: we are given one life and the decision is ours whether to wait for circumstances to make up our mind, or whether to act and, in acting, to live.
Everyone has his day and some days last longer than others.
No one really knows enough to be a pessimist.
Hard reality has a way of cramping your style.
The world is all gates, all opportunities, strings of tension waiting to be struck.
A new idea is delicate.
It can be killed by a sneer or a yawn; it can be stabbed to death by a joke or worried to death by a frown on the right person's brow.
Kites rise highest against the wind - not with it.
There's no success like failure, and failure's no success at all.
I avoid looking forward or backward, and try to keep going forward.
You don't drown by falling in the water; you drown by staying there.
Success is the sum of small efforts, repeated day in and day out.
I have learned to use the word ""impossible"" with the greatest caution.
No pressure, no diamonds.
Intelligence is nothing without delight.
Man usually avoids attributing cleverness to somebody else - unless it is an enemy.
Whether you think that you can or that you can't, you are usually right.
If all else fails, immortality can always be assured by spectacular error.
A celebrity is a person who is known for his well-knownness.
The secret of success is constancy to purpose.
The secret to creativity is knowing how to hide your sources.
It is easier for a camel to pass through the eye of a needle if it is lightly greased.
The man who follows the crowd will usually get no further than the crowd.
The man who walks alone is likely to find himself in places no one has ever been.
Luck can't last a lifetime unless you die young.
A conclusion is simply the place where someone got tired of thinking.
He who thinks he is raising a mound may only in reality be digging a pit.
Why be a man when you can be a success?
A pessimist sees the difficulty in every opportunity; an optimist sees the opportunity in every difficulty.
Self-respect permeates every aspect of your life.
An empowered organization is one in which individuals have the knowledge, skill, desire, and opportunity to personally succeed in a way that leads to collective organizational success.
Hitch your wagon to a star.
To accomplish great things, you must not only act but also dream, not only dream but also believe.
Advertising is 85% confusion and 15% commission.
Misfortune: The kind of fortune that never misses.
When you reach for the stars, you may not quite get one, but you won't come up with a handful of mud either.
Every man is the architect of his own fortune.
A man who has committed a mistake and doesn't correct it, is committing another mistake.
When I discover who I am, I'll be free.
Inform all the troops that communications have completely broken down.
If at first you don't succeed, try, and try again.
Then give up.
Genius is one per cent inspiration and ninetynine per cent perspiration.
You can't build a reputation on what you are going to do.
In labouring to be brief, I become obscure.
It is neither wealth nor splendour, but tranquillity and occupation, that gives happiness.
A journey of a thousand miles must begin with a single step.
A scholar who cherishes the love of comfort is not fit to be deemed a scholar.
We judge ourselves by what we feel capable of doing, while others judge us by what we have done.
People forget how fast you did a job - but they remember how well you did it.
Until you value yourself, you will not value your time.
Until you value your time, you will not do anything with it.
Peter's Principle: In an organization, each person rises to the level of his own incompetence.
A committee is a thing which takes a week to do what one good man can do in an hour.
I find that the harder I work, the more luck I seem to have.
People who work sitting down are paid more than people who work standing up.
Work is only work if you'd rather be doing something else.
Nobody realizes that some people expend tremendous energy merely to be normal.
There's no real need to do housework - after four years it doesn't get any worse.
Hell, there are no rules here - we're trying to accomplish something.
Committee: A group of the unwilling, picked from the unfit to do the unnecessary.
If food were free, why work?
Leemans' Law: Junk expands to fill the space allotted.
I can write better than anybody who can write faster, and I can write faster than anybody who can write better.
Ambition is a poor excuse for not having sense enough to be lazy.
He who rocks the boat seldom has time to row it.
Don't tell people how to do things.
Tell them what to do and let them surprise you with their results.
Originality is the fine art of remembering what you hear but forgetting where you heard it.
Whatever you do will be insignificant, but it is very important that you do it.
We will burn that bridge when we come to it.
Anyone can do any amount of work provided it isn't the work he is supposed to be doing at the moment.
He who desires, but acts not, breeds pestilence.
If there is no struggle, there is no progress.
Well done is better than well said.
The only way round is through.
For a man to achieve all that is demanded of him, he must regard himself as greater than he is.
Trouble is only an opportunity in work clothes.
Parkinson's First Law: Work expands to fill the time available.
There is nothing so easy but that it becomes difficult when you do it reluctantly.
All paid jobs absorb and degrade the mind.
Try to relax and enjoy the crisis.
Iron rusts from disuse, stagnant water loses its purity, and in cold weather becomes frozen, even so does inaction sap the vigours of the mind.
Efficiency is intelligent laziness.
It is time I stepped aside for a less experienced and less able man.
My work is a game - a very serious game.
Most problems are either unimportant or impossible to solve.
If one has not given everything, one has given nothing.
It is by will alone that I set my mind in motion.
Genius is an infinite capacity for taking pains.
He has half the deed done who has made a beginning.
People who never do any more than they get paid for never get paid for any more than they do.
The best way to predict the future is to invent it.
It gets late early out there.
Never mistake motion for action.
Every day I get up and look through the Forbes list of the richest people in America.
If I'm not there, I go to work.
A conference is just an admission that you want somebody to join you in your troubles.
Too much credit is given to the end result.
The true lesson is in the struggle that takes place between the dream and reality.
That struggle is a thing called life!
Talk is cheap because supply exceeds demand.
A youth becomes a man when the marks he wants to leave on the world have nothing to do with tyres.
Analyzing humor is like dissecting a frog: nobody enjoys it, and the frog usually dies as a result.
Don't think of it as being outnumbered, think of it as a wide target selection.
Everything should be made as simple as possible, but not simpler.
No vacation goes unpunished.
Utility is when you have one telephone, luxury is when you have two, and paradise is when you have none.
Even if you're on the right track, you'll get run over if you just sit there.
Farming looks easy when your plough is a pencil and you're a thousand miles from a cornfield.
Everybody wants to save the earth; nobody wants to help mother do the dishes.
Do what you can, with what you have, where you are.
What is written without effort is in general read without pleasure.
I'm a slow walker, but I never walk back.
There is nothing more difficult to take in hand, more perilous to conduct or more uncertain in its success than to take the lead in the introduction of a new order of things.
Not to be able to bear poverty is a shameful thing, but not to know how to chase it away by work is a more shameful thing yet.
I love deadlines.
I like the whooshing sound they make as they fly by.
I want to be what I was when I wanted to be what I am now.
Necessity is the mother of invention.
Consistency is the final refuge of the unimaginative.
A snooze button is a poor substitute for no alarm clock at all.
Only those who you trust can betray you.
A baby is something you carry inside of you for 9 months, In your arms for three years, And in your heart till the day you die.
The only failure without dignity is the failure to try.
In spite of the costs of living, it's still popular.
Suspicion always haunts the guilty mind.
There's no trick to being a humourist when you have the whole government working for you.
It's easy if you try.
Climate is what we expect, weather is what we get.
A committee is a life form with six or more legs and no brain.
Just tell us where it is; we'll find it.
Medicine reports that it has already happened.
Let's not disappoint him.
The only thing that separates us from the animals is mindless superstition and pointless rituals.
Men never do evil so cheerfully and so completely as when they do so from religious conviction.
Last night I dreamed I ate a ten-pound marshmallow, and when I woke up the pillow was gone.
I either want less corruption, or more chance to participate in it.
Doing a thing well is often a waste of time.
He is one of those people who would be enormously improved by death.
The secret of success is sincerity.
Once you can fake that, you've got it made.
Boy, everyone is stupid except me.
When doctors and undertakers meet, they always wink at each other.
The best cure for insomnia is plenty of sleep.
A happy person is not a person in a certain set of circumstances, but rather a person with a certain set of attitudes.
Happiness is a positive cash flow.
A good day is when you wake up without a chalk outline around your body.
It is well to remember that the entire universe, with one trifling exception, is composed of others.
The hardest thing about any political campaign is how to win without proving that you are unworthy of winning.
What's another word for Thesaurus?
I always turn to the sports page first, which record people's accomplishments.
To err is human; to admit it is not.
A low voter turnout is an indication of fewer people going to the polls.
The future will be better tomorrow.
A straight line may be the shortest distance between two points, but it is by no means the most interesting.
And which parallel universe did you crawl out of?
Be careful about reading health books.
You might die of a misprint.
We can lick gravity, but sometimes the paperwork is overwhelming.
Experience is that marvellous thing that enables you recognize a mistake when you make it again.
No one is perfect.
The mere fact that one is human is a flaw in itself.
Things are more like they are now than they ever were before.
The only thing you can do easily is be wrong, and that's hardly worth the effort.
When we are planning for posterity, we ought to remember that virtue is not hereditary.
Shoot for the stars, otherwise gravity gets in your way.
We don't want to go back to tomorrow, we want to go forward.
Verbosity leads to unclear, inarticulate things.
I believe we are on an irreversible trend toward more freedom and democracy - but that could change.
I was recently on a tour of Latin America, and the only regret I have was that I didn't study Latin harder in school so I could converse with those people.
No god wants to accidentally smite the wrong person and get sued by another god.
If your mind changes itself fast enough, the result is vertigo.
Not wrong in particulars, but wrong in general, wrong about everything.
The right to be heard does not automatically include the right to be taken seriously.
Try calling up strangers in the phone book and forgiving them.
Not only will it loosen you up for the crucial real thing, the strangers will feel better.
Everyone likes to know they're forgiven.
Never do today what you can do tomorrow.
Never ruin an apology with an excuse.
Never give up.
And never, under any circumstances, face the facts.
Never stand between a dog and the hydrant.
The reason most people play golf is to wear clothes they would not be caught dead in otherwise.
I would love to speak a foreign language but I can't.
I'm not into working out.
My philosophy: No pain, no pain.
It's like deja vu all over again.
This is one race of people for whom psychoanalysis is of no use whatsoever.
Lots of comedians have people they try to mimic.
I mimic my shadow.
I put tape on the mirrors in my house so I don't accidentally walk through into another dimension.
I've been doing a lot of abstract painting lately, extremely abstract.
No brush, no paint, no canvas, I just think about it.
My watch is three hours fast, and I can't fix it.
So I'm going to move to New York.
I like to reminisce with people I don't know.
I like to skate on the other side of the ice.
If you can't hear me, it's because I'm in parentheses.
Is it weird in here, or is it just me?
Every so often, I like to stick my head out the window, look up, and smile for a satellite picture.
I'm moving to Mars next week, so if you have any boxes.
Sorry, my mind was wandering.
One time my mind went all the way to Venus on mail order and I couldn't pay for it.
It's a small world, but I wouldn't want to have to paint it.
Cross country skiing is great if you live in a small country.
You can't have everything.
Where would you put it?
It's a good thing we have gravity, or else when birds died they'd just stay right up there.
When I die, I'm leaving my body to science fiction.
I saw a bank that said ""24 Hour Banking"", but I don't have that much time.
I went to the museum where they had all the heads and arms from the statues that are in all the other museums.
On the other hand the early worm gets eaten.
Right now I'm having amnesia and deja vu at the same time.
I think I've forgotten this before.
If you're not part of the solution, you're part of the precipitate.
I bought some powdered water, but I don't know what to add.
I spilled spot remover on my dog.
He's gone now.
Outside of a dog, a book is man's best friend.
Quote me as saying I was misquoted.
I must confess, I was born at a very early age.
Military justice is to justice what military music is to music.
Are you any relation to your brother Marv?
I've never had major knee surgery on any other part of my body.
Smoking kills.
I was a pilot flying an airplane and it just so happened that where I was flying made what I was doing spying.
A personal injustice is stronger motivation than any instinct for philanthropy.
There are two kinds of people, those who finish what they start and so on.
Serious people have few ideas.
People with ideas are never serious.
Human beings, who are almost unique in having the ability to learn from the experience of others, are also remarkable for their apparent disinclination to do so.
The only way to keep your health is to eat what you don't want, drink what you don't like and do what you'd rather not.
The world's a stage and most of us are desperately unrehearsed.
If you can't live without me, why aren't you dead already?
A dog teaches a boy fidelity, perseverance and to turn round three times before lying down.
A women's work is never done by men.
When a man brings his wife flowers for no reason - there's a reason.
Martyrdom is the only way in which a man can become famous without ability.
The surest way to make a monkey out of a man is to quote him.
A kleptomaniac is a person who helps himself because he can't help himself.
It usually takes more than three weeks to prepare a good impromptu speech.
Somebody's boring me.
I think it's me.
Abstention is a vote in favour of the oppressor.
It is my ambition to say in ten sentences what others say in a whole book.
Man is still the most extraordinary computer of all.
In the realm of human destiny, the depth of man's questioning is more important than his answers.
Maturity is a bitter disappointment for which no remedy exists, unless laughter could be said to remedy anything.
If you cannot get rid of the family skeleton, you may as well make it dance.
I have seen hypocrisy that was so artful that it was good judgment to be deceived by it.
Morality is simply the attitude we adopt towards people whom we dislike.
Never raise your hands to your kids.
The second day of a diet is always easier than the first.
By the second day you're off it.
Lazlo's Chinese Relativity Axiom: No matter how great your triumphs or how tragic your defeats - approximately one billion Chinese couldn't care less.
When cryptography is outlawed, bayl bhgynjf jvyy unir cevinpl.
Never underestimate the bandwidth of a station wagon full of tapes hurtling down the highway.
Karate is a form of martial arts in which people who have had years and years of training can, using only their hands and feet, make some of the worst movies in the history of the world.
Applying computer technology is simply finding the right wrench to pound in the correct screw.
Technology is a way of organizing the universe so that man doesn't have to experience it.
In answer to the question of why it happened, I offer the modest proposal that our Universe is simply one of those things which happen from time to time.
Only two things are infinite, the universe and human stupidity, and I'm not sure about the former.
When I was a kid, all we had to do was just sit around and hope somebody would invent television so we could play Nintendo.
History doesn't repeat itself - historians merely repeat each other.
In any collection of data, the figure most obviously correct, beyond all need of checking, is the mistake.
The first bug to hit a clean windshield lands directly in front of your eyes.
If you hit two keys on the typewriter, the one you don't want hits the paper.
If computers get too powerful, we can organize them into a committee - that will do them in.
When it comes to music lessons, most kids make it a practice not to practice.
When we seek to discover the best in others, we somehow bring out the best in ourselves.
When you fool a fool you strike a blow for intelligence.
When your dreams turn to dust, vacuum.
Where love leads, happiness follows.
Who says nothing is impossible, I have been doing nothing for years.
Whoever said you can't buy happiness forgot about puppies.
Somewhere, something incredible is waiting to be known.
Success is the result of perfection, hard work, learning from failure, loyalty, and persistence.
Take time to laugh - it is the music of the soul.
You can't shake hands with a clenched fist.
You cannot acquire experience by making experiments.
You cannot create experience.
You must undergo it.
Sometimes you gotta laugh through the tears, smile through the pain so that you can live through the sorrow.
Standard mathematics has recently been rendered obsolete by the discovery that for years we have been writing the numeral five backward.
This has led to reevaluation of counting as a method of getting from one to ten.
Students are taught advanced concepts of Boolean algebra, and formerly unsolvable equations are dealt with by threats of reprisals.
Stress is when you wake up screaming and you realize you haven't fallen asleep yet.
Success is the best revenge.
Table manners must have been invented by people who were never hungry.
Take away love and earth is a tomb.
Tear is a powerful weapon that can change the future of oneself or even the world.
The trouble with the rat race is that even if you win, you're still a rat.
The nice thing about meditation is that it makes doing nothing quite respectable.
Happiness is having a large, loving, caring, close-knit family in another city.
Of course there's a lot of knowledge in universities: the freshmen bring a little in; the seniors don't take much away, so knowledge sort of accumulates.
My mother's menu consisted of two choices: Take it or leave it.
When I give a lecture, I accept that people look at their watches, but what I do not tolerate is when they look at it and raise it to their ear to find out if it stopped.
In politics, merit is rewarded by the possessor being raised, like a target, to a position to be fired at.
But as empty gestures go, it is one of the best.
Reject hatred without hating.
Bigot: One who is obstinately and zealously attached to an opinion that you do not entertain.
Be bad while you are young - then you can spend the rest of your life repenting and improving.
Alliance: In international politics, the union of two thieves who have their hands so deeply inserted into each others' pockets that they cannot separately plunder a third.
Eulogy: Praise of a person who has either the advantages of wealth and power, or the consideration to be dead.
It's usually the day after she reminds me about it.
When I have a birthday I take the day off.
We know when we're getting old when the only thing we want for our birthday is not to be reminded of it.
It's so sad to grow old alone.
By the time the last candle was lit on her birthday cake in February, the first one had gone out.
If she ever told her real age her birthday cake would be a fire hazard.
When it was fully lit it looked like a prairie fire.
I took an IQ test and the results were negative.
If everything is coming your way, then you're in the wrong lane.
I just love nonverbal communication!
You can't be late until you show up.
The report of my death was an exaggeration.
It's funny how most people love the dead, once you're dead you're made for life.
When I was a boy the Dead Sea was only sick.
They say such nice things about people at their funerals that it makes me sad that I'm going to miss mine by just a few days.
When you've told someone that you've left them a legacy the only decent thing to do is to die at once.
You know you're getting old when you stoop to tie your shoelaces and wonder what else you could do while you're down there.
My grandmother was a very tough woman.
On the plus side, death is one of the few things that can be done as easily lying down.
People ask me what I'd most appreciate getting for my eighty-seventh birthday.
I tell them, a paternity suit.
At my age flowers scare me.
The reason a dog has so many friends is that he wags his tail instead of his tongue.
Don't accept your dog's admiration as conclusive evidence that you are wonderful.
The average dog is a nicer person than the average person.
No animal should ever jump up on the dining-room furniture unless absolutely certain that he can hold his own in the conversation.
If I have any beliefs about immortality, it is that certain dogs I have known will go to heaven, and very, very few persons.
A dog teaches a boy fidelity, perseverance, and to turn around three times before lying down.
I wonder if other dogs think poodles are members of a weird religious cult.
Outside of a dog, a book is probably man's best friend; inside of a dog, it's too dark to read.
Either he's dead or my watch has stopped.
I don't want to achieve immortality through my work, I want to achieve it through not dying.
All good things in life are either immoral, fattening or overpriced.
If you go back in time, don't step on anything.
The reports of my death have been greatly exaggerated.
Before you criticize someone, you should walk a mile in their shoes.
That way, when you criticize them, you are a mile away from them, and you have their shoes.
A horror movie without the horror is like the turkey sandwich without the Miracle Whip.
I can't decide if indecision is good or bad.
Imagine if there were no hypothetical situations.
Drive-in banks were established so most of the cars today could see their real owners.
Nobody goes where the crowds are anymore.
It's too crowded.
When they asked George Washington for his ID, he just took out a quarter.
Why do people sing ""Take Me Out to the Ballgame"" when they're already there?
When you come to a fork in the road, take it.
Too bad that all the people who know how to run the country are driving taxi cabs and cutting hair.
Conscience is the inner voice warning us that someone may be looking.
Always carry a flagon of whisky in case of snakebite and furthermore always carry a small snake.
What if nothing exists and we're all in somebody's dream?
Or what's worse, what if only that fat guy in the third row exists?
Everybody is ignorant, only on different subjects.
If the facts don't fit the theory, change the facts.
Better to remain silent and be thought a fool, than to speak and remove all doubt.
The best way to remember your wife's birthday is to forget it once.
To be sure of hitting the target, shoot first, and call whatever you hit the target.
I always wanted to be somebody, but I should have been more specific.
I have lost friends, some by death, others through sheer inability to cross the street.
It is amazing what you can accomplish if you do not care who gets the credit.
The main purpose of the stock market is to make fools of as many men as possible.
No snowflake in an avalanche ever feels responsible.
Honesty is the key to a relationship.
If you can fake that, you're in.
How much patience you have for instance.
Honest criticism is hard to take, particularly from a relative, a friend, an acquaintance, or a stranger.
I don't believe in an afterlife, so I don't have to spend my whole life fearing hell, or fearing heaven even more.
For whatever the tortures of hell, I think the boredom of heaven would be even worse.
Work like you don't need the money.
Love like you've never been hurt.
Dance like nobody's watching.
Never interrupt your enemy when he is making a mistake.
Experience is that marvellous thing that enables you to recognize a mistake when you make it again.
First they ignore you, then they laugh at you, then they fight you, then you win.
A fanatic is a person who can't change his mind and won't change the subject.
To invent, you need a good imagination and a pile of junk.
You can pretend to be serious; you can't pretend to be witty.
Instead of giving a politician the keys to the city, it might be better to change the locks.
The person who knows how to laugh at himself will never cease to be amused.
One of the symptoms of an approaching nervous breakdown is the belief that one's work is terribly important.
If you're going to do something tonight that you'll be sorry for tomorrow, sleep late.
It isn't necessary to be rich and famous to be happy.
It's only necessary to be rich.
Moral indignation is jealousy with a halo.
Even if it doesn't work, there is something healthy and invigorating about direct action.
A gossip is one who talks to you about others, a bore is one who talks to you about himself; and a brilliant conversationalist is one who talks to you about yourself.
I figure you have the same chance of winning the lottery whether you play or not.
A black cat crossing your path signifies that the animal is going somewhere.
Radio news is bearable.
This is due to the fact that while the news is being broadcast the disc jockey is not allowed to talk.
From the moment I picked your book up until I laid it down I was convulsed with laughter.
Some day I intend reading it.
Better lose to gain but never win to lose.
Empathise empathy emphatically in your approach to every interpersonal relationship.
A loving heart is the truest wisdom.
Love is a fruit in season at all times, and in reach of every hand.
Love many things, for therein lies the true strength, and whosoever loves much performs much, and can accomplish much, and what is done in love is done well.
Every heart sings a song, incomplete, until another heart whispers back.
Those who wish to sing always find a song.
Being deeply loved by someone gives you strength, while loving someone deeply gives you courage.
Absence diminishes small loves and increases great ones, as the wind blows out the candle and blows up the bonfire.
The greatest happiness of life it the conviction that we are loved - loved for ourselves, or rather, loved in spite of ourselves.
You yourself, as much as anybody in the entire universe, deserve your love and affection.
Give me ambiguity or give me something else.
We are born naked, wet and hungry; then things get worse.
He who laughs last thinks slowest!
Always remember you're unique, just like everyone else.
Lottery: A tax on people who are bad at math.
Hard work has a future payoff.
Friends help you move.
Puritanism: The haunting fear that someone, somewhere may be happy.
I used to have a handle on life, then it broke.
Depression is merely anger without enthusiasm.
Eagles may soar, but weasels don't get sucked into jet engines.
Early bird gets the worm, but the second mouse gets the cheese.
I'm not cheap, but I am on special this week.
I almost had a psychic girlfriend but she left me before we met.
I drive way too fast to worry about cholesterol.
I intend to live forever - so far, so good.
Mental backup in progress - Do Not Disturb!
The only substitute for good manners is fast reflexes.
When everything's coming your way, you're in the wrong lane.
Ambition is a poor excuse for not having enough sense to be lazy.
If I worked as much as others, I would do as little as they.
If everything seems to be going well, you have obviously overlooked something.
Many people quit looking for work when they find a job.
Dancing is a perpendicular expression of a horizontal desire.
All those who believe in psychogenesis raise my hand.
A clean desk is a sign of a cluttered desk drawer.
The Universe is a figment of its own imagination.
Why was the Tomato blushing?
Because he saw the salad dressing.
How do you catch a squirrel?
Climb into a tree and act like a nut.
Why didn't the skeleton cross the road?
Because he/she had no guts!
Why didn't the chicken cross the road?
Because he was too chicken.
What lies at the bottom of the ocean and twitches?
A nervous wreck!
Why don't cannibals eat comedians?
Because they taste funny.
What's brown and sticky?
What does Mozart do now that he is dead?
Why do they put bells on cows?
Because their horns don't work!
Why did the bee cross his legs?
Because he couldn't find the BP station.
What do you call a missing parrot?
A polygon.
Where does a one armed man shop?
At a second hand store!
What do you call a sleeping cow?
A bulldozer.
Did you hear about the cat who swallowed a ball of yarn?
She had mittens!
Why can't skeletons play music in church?
They have no organs!
What's brown and sounds like a bell?
What bird can lift the most weight?
Why was the man arrested for waiting in the Big Top?
He was loitering within tent.
Because he saw his phone bill.
Why were all the ink spots crying?
Their father was in the pen.
There's no future in time travel.
Tonight's weather: Dark with continued darkness until dawn.
Death is hereditary.
Multitasking - screwing up several things at once.
Beat the 5 o'clock rush - Leave work at noon!
Arachibutyrophobia: fear of peanut butter sticking to roof of mouth.
Polynesia: memory loss in parrots.
A good pun is its own reword.
Friends may come and go, but enemies tend to accumulate.
A conclusion is the place where you got tired of thinking.
Experience is something you don't get until just after you need it.
For every action, there is an equal and opposite criticism.
He who hesitates is probably right.
Never do card tricks for the group you play poker with.
No one is listening until you make a mistake.
The colder the X-ray table, the more of your body is required on it.
The hardness of the butter is proportional to the softness of the bread.
The severity of the itch is proportional to the reach.
To succeed in politics, it is often necessary to rise above your principles.
Two wrongs are only the beginning.
You never really learn to swear until you learn to drive.
The problem with the gene pool is that there is no lifeguard.
A clear conscience is usually the sign of a bad memory.
If you must choose between two evils, pick the one you've never tried before.
A fool and his money are soon partying.
Hell hath no fury like the lawyer of a woman scorned.
Bills travel through the mail at twice the speed of checks.
Hard work pays off in the future.
Eagles may soar, but weasels aren't sucked into jet engines.
Borrow money from pessimists - they don't expect it back.
Half the people you know are below average.
A conscience is what hurts when all your other parts feel so good.
If at first you don't succeed, then skydiving definitely isn't for you!
OK, so what's the speed of dark?
Wisdom and beauty form a very rare combination.
Education: That which discloses to the wise and disguises from the fool their lack of understanding.
Vote: The instrument and symbol of a free man's power to make a fool of himself and a wreck of his country.
To be sure of hitting the target, shoot first and whatever you hit, call it the target.
I was educated once, and it took me years to get over it.
The surest way to remain a winner is to win once, and then not play any more.
Politics is the gentle art of getting votes from the poor and campaign funds from the rich by promising to protect each from the other.
Riot: A popular entertainment given to the military by innocent bystanders.
It is not the bad times on which we should dwell, it is only poison to the mind and soul.
We shall rise up after we fall, and continue to go on - dwelling on the good, high-spirited times of our lives.
It is a sure sign that you are alive.
If what Proust says is true, that happiness is the absence of fever, then I will never know happiness.
For I am possessed by a fever for knowledge, experience, and creation.
The heart has its reasons that reason knows nothing of.
No one can make you feel inferior without your consent.
Trouble is part of your life - if you don't share it, you don't give the person who loves you a chance to love you enough.
He has achieved success who has lived well, laughed often, and loved much.
Comedy is simply a funny way of being serious.
Man will occasionally stumble over the truth, but most of the time he will pick himself up and continue on.
Any sufficiently advanced technology is indistinguishable from a rigged demo.
I predict that exact reproduction through cloning will not become popular.
Too many people already find it difficult to live with themselves.
Every man is a divinity in disguise, a god playing the fool.
Disorder increases with time because we measure time in the direction in which disorder increases.
I don't necessarily agree with everything I say.
Nothing is more intolerable than to have to admit to yourself your own errors.
Man is able to do what he is unable to imagine.
His head trails a wake through the galaxy of the absurd.
I like pigs.
Dogs look up to us.
Cats look down on us.
Pigs treat us as equals.
Most of life is choices, and the rest is pure dumb luck.
Failure is the opportunity to begin again more intelligently.
I have made mistakes, but have never made the mistake of claiming I never made one.
It's not over until it's over.
Nothing great was ever achieved without enthusiasm.
Don't worry about people stealing your ideas.
If your ideas are any good, you'll have to ram them down people's throats.
The brain is a wonderful organ.
It starts working the moment you get up in the morning and does not stop until you get to work.
The quality of an organization can never exceed the quality of the minds that make it up.
Parkinson's Fourth Law: The number of people in any working group tends to increase regardless of the amount of work to be done.
You will break the bow if you keep it always stretched.
Give me a museum and I'll fill it.
The beginning is the most important part of the work.
He that would perfect his work must first sharpen his tools.
Tell him a bench has wet paint and he has to touch it.
While others may go hungry, we've eaten very well.
With home, health and happiness, I shouldn't want to fuss.
The act of putting pen to paper encourages pause for thought, this in turn makes us think more deeply about life, which helps us regain our equilibrium.
The best thing to sleep on is a clear conscience.
The computer only crashes when printing a document you haven't saved.
The hardest thing in the world to understand is the income tax.
The last day of school before summer vacation is the shortest day of a mother's year.
The life you have led doesn't need to be the only life you have.
The more we learn, the more we realize how little we know.
The only abnormality is the incapacity to love.
The past is but the beginning of a beginning, and all that is and has been is but the twilight of the dawn.
Of all the things which wisdom provides to make life entirely happy, much the greatest is the possession of friendship.
Every now and again take a good look at something not made with hands - a mountain, a star, the turn of a stream.
There will come to you wisdom and patience and solace and, above all, the assurance that you are not alone in the world.
Friendship is the hardest thing in the world to explain.
It's not something you learn in school.
But if you haven't learned the meaning of friendship, you really haven't learned anything.
I am not in this world to live up to other people's expectations, nor do I feel that the world must live up to mine.
Friendship is precious, not only in the shade, but in the sunshine of life; and thanks to a benevolent arrangement of things, the greater part of life is sunshine.
Life is no brief candle to me.
It is a sort of splendid torch which I have got a hold of for the moment, and I want to make it burn as brightly as possible before handing it on to future generations.
How far you go in life depends on your being tender with the young, compassionate with the aged, sympathetic with the striving, and tolerant of the weak and strong.
Because someday in your life you will have been all of these.
Live all you can.
It is a mistake not to.
It doesn't matter so much what you do in particular, so long as you have your life.
If you haven't had that, what have you had?
No one gets out of this world alive, so the time to live, learn, care, share, celebrate, and love is now.
There is no cure for birth and death save to enjoy the interval.
Love is a state in which a man sees things most decidedly as they are not.
Love wasn't put in your heart to stay.
Love isn't love 'til you give it away.
Let love be your greatest aim.
If thou must love me, let it be for naught except for love's sake only.
To live without loving is not really to live.
At the touch of love, everyone becomes a poet.
The world is indeed full of peril, and in it there are many dark places; but still there is much that is fair, and though in all lands love is mingled with grief, love grows perhaps the greater.
True love doesn't consist of holding hands, it consists of holding hearts.
Age does not protect you from love.
But love, to some extent, protects you from age.
Lots of people are willing to die for the person they love, which is a pity, for it is a much grander thing to live for that person.
It is a curious thought, but it is only when you see people looking ridiculous that you realize just how much you love them.
The supreme happiness in life is the conviction that we are loved.
Don't be so humble - you are not that great.
Glory is fleeting, but obscurity is forever.
The school of hard knocks is an accelerated curriculum.
I am always doing things I can't do, that's how I get to do them.
Nagging is the repetition of unpalatable truths.
The dreams in my spirit mirror the solitude of my dark solitude.
No computer has ever been designed that is ever aware of what it's doing; but most of the time, we aren't either.
Accept no one's definition of your life, but define yourself.
Press any key to continue, or any other key to quit.
Close your eyes and press escape three times.
This will end your Windows session.
Do you want to play another game?
The E-mail of the species is more deadly than the mail.
A journey of a thousand sites begins with a single click.
You can't teach a new mouse old clicks.
Great groups from little icons grow.
Speak softly and carry a cellular phone.
Don't put all your hypes in one home page.
The modem is the message.
Too many clicks spoil the browse.
A chat has nine lives.
Don't byte off more than you can view.
Fax is stranger than fiction.
What boots up must come down.
Virtual reality is its own reward.
Modulation in all things.
A user and his leisure time are soon parted.
Know what to expect before you connect.
Oh, what a tangled web-site we weave, when first we practice.
All I want is a warm bed and a kind word and unlimited power.
An expert is one who knows more and more about less and less until he knows absolutely everything about nothing.
By working faithfully eight hours a day, you may eventually get to be boss and work twelve.
Copy from one, it's plagiarism; copy from two, it's research.
Corduroy pillows: They're making headlines!
Everyone has a photographic memory.
Everywhere is walking distance if you have the time.
Failure to prepare is preparing to fail.
For Sale: Parachute.
Great spirits have always encountered violent opposition from mediocre minds.
How do you tell when you run out of invisible ink?
I couldn't repair your brakes, so I made your horn louder.
I don't have any solution but I certainly admire the problem.
I used to have an open mind but my brains kept falling out.
I'm writing a book.
I've got the page numbers done.
If at first you don't succeed, destroy all evidence that you tried.
If you're killed, you've lost a very important part of your life.
It's not that I'm afraid to die, I just don't want to be there when it happens.
Knowledge and belief are two separate tracks that run parallel to each other and never meet, except in the child.
Laughing stock: cattle with a sense of humor.
Man is only happy as he finds a work worth doing, and does it well.
Most plans are just inaccurate predictions.
My opinions may have changed, but not the fact that I am right.
My schedule is already full.
Opportunity is missed by most people because it is dressed in overalls and looks like work.
Research is what I'm doing when I don't know what I'm doing.
Shin: a device for finding furniture in the dark.
Some don't have film.
Success always occurs in private, and failure in full view.
Support your right to bare arms!
That's relativity.
The more I want to get something done, the less I call it work.
The reward of a thing well done is to have done it.
The trouble with our times is that the future is not what it used to be.
There cannot be a crisis next week.
Truth comes out of error more easily than out of confusion.
We would often be sorry if our wishes were gratified.
Wear short sleeves!
What happens if you get scared half to death twice?
Who is General Failure and why is he reading my hard disk?
Why do psychics have to ask you for your name?
You can learn many things from children.
There was Gertrude.
And there was his sister Marian.
Norman and Arthur knew that speech.
He had heard them talking it.
And they were her brothers.
He left the alcove in despair.
From every side the books seemed to press upon him and crush him.
He had never dreamed that the fund of human knowledge bulked so big.
He was frightened.
How could his brain ever master it all?
And so he wandered on, alternating between depression and elation as he stared at the shelves packed with wisdom.
In a way, it spoke a kindred speech.
Both he and it were of the sea.
There it was; he would teach himself navigation.
He would quit drinking, work up, and become a captain.
Ruth seemed very near to him in that moment.
As a captain, he could marry her (if she would have him).
He cast his eyes about the room and closed the lids down on a vision of ten thousand books.
No; no more of the sea for him.
There was power in all that wealth of books, and if he would do great things, he must do them on the land.
Besides, captains were not allowed to take their wives to sea with them.
Noon came, and afternoon.
But when he found the right shelf, he sought vainly for the answer.
He abandoned his search.
The man nodded.
Are you a sailor?
""Yes, sir,"" he answered.
Now, how did he know that?
A terrible restlessness that was akin to hunger afflicted Martin Eden.
He could not steel himself to call upon her.
He was afraid that he might call too soon, and so be guilty of an awful breach of that awful thing called etiquette.
The many books he read but served to whet his unrest.
Every page of every book was a peep-hole into the realm of knowledge.
His hunger fed upon what he read, and increased.
Also, he did not know where to begin, and continually suffered from lack of preparation.
The commonest references, that he could see plainly every reader was expected to know, he did not know.
And the same was true of the poetry he read which maddened him with delight.
He read more of Swinburne than was contained in the volume Ruth had lent him; and ""Dolores"" he understood thoroughly.
But surely Ruth did not understand it, he concluded.
These thoughts he had tried to share, but never had he found a woman capable of understanding - nor a man.
He had tried, at times, but had only puzzled his listeners.
And as his thoughts had been beyond them, so, he argued now, he must be beyond them.
He felt power move in him, and clenched his fists.
Those bold black eyes had nothing to offer.
He knew the thoughts behind them - of ice-cream and of something else.
They offered books and painting, beauty and repose, and all the fine elegance of higher existence.
Behind those black eyes he knew every thought process.
It was like clockwork.
He could watch every wheel go around.
Their bid was low pleasure, narrow as the grave, that palled, and the grave was at the end of it.
He had caught glimpses of the soul in them, and glimpses of his own soul, too.
""Lizzie,"" she replied, softening toward him, her hand pressing his arm, while her body leaned against his.
He talked on a few minutes before saying good night.
I kept it for you.
A week of heavy reading had passed since the evening he first met Ruth Morse, and still he dared not call.
Time and again he nerved himself up to call, but under the doubts that assailed him his determination died away.
But his eyes were strong, and they were backed by a body superbly strong.
Furthermore, his mind was fallow.
It had never been jaded by study, and it bit hold of the knowledge in the books with sharp teeth that would not let go.
It seemed to him, by the end of the week, that he had lived centuries, so far behind were the old life and outlook.
But he was baffled by lack of preparation.
He attempted to read books that required years of preliminary specialization.
It was the same with the economists.
He was bewildered, and yet he wanted to know.
She had never had any experiences of the heart.
She did not know the actual fire of love.
She did not dream of the volcanic convulsions of love, its scorching heat and sterile wastes of parched ashes.
She knew neither her own potencies, nor the potencies of the world; and the deeps of life were to her seas of illusion.
It was only natural.
There was something cosmic in such things, and there was something cosmic in him.
He came to her breathing of large airs and great spaces.
She read Browning aloud to him, and was often puzzled by the strange interpretations he gave to mooted passages.
In an immediate way it personified his life.
But her singing he did not question.
It was too wholly her, and he sat always amazed at the divine melody of her pure soprano voice.
She knew her Browning, but it had never sunk into her that it was an awkward thing to play with souls.
As her interest in Martin increased, the remodelling of his life became a passion with her.
Butler, Charles Butler he was called, found himself alone in the world.
His father had come from Australia, you know, and so he had no relatives in California.
She paused for breath, and to note how Martin was receiving it.
Butler; but there was a frown upon his face as well.
How could he live on it?
He must have lived like a dog.
His early denials are paid for a thousand fold.
Martin looked at her sharply.
Her eyes dropped before his searching gaze.
By nature he is sober and serious.
He always was that.
""You can bet he was,"" Martin proclaimed.
Her own limits were the limits of her horizon; but limited minds can recognize limitations only in others.
""But I have not finished my story,"" she said.
Butler was always eager to work.
He never was late, and he was usually at the office a few minutes before his regular time.
And yet he saved his time.
Every spare moment was devoted to study.
He quickly became a clerk, and he made himself invaluable.
Father appreciated him and saw that he was bound to rise.
He became a lawyer, and hardly was he back in the office when father took him in as junior partner.
He is a great man.
Such a life is an inspiration to all of us.
It shows us that a man with will may rise superior to his environment.
""He is a great man,"" Martin said sincerely.
And then, in splendour and glory, came the great idea.
He would write.
He would write - everything - poetry and prose, fiction and description, and plays like Shakespeare.
There was career and the way to win to Ruth.
Butlers who earned thirty thousand a year and could be Supreme Court justices if they wanted to.
Once the idea had germinated, it mastered him, and the return voyage to San Francisco was like a dream.
He was drunken with unguessed power and felt that he could do anything.
In the midst of the great and lonely sea he gained perspective.
Clearly, and for the first lime, he saw Ruth and her world.
The thought was fire in him.
He would begin as soon as he got back.
The first thing he would do would be to describe the voyage of the treasure-hunters.
He would sell it to some San Francisco newspaper.
He would not tell Ruth anything about it, and she would be surprised and pleased when she saw his name in print.
While he wrote, he could go on studying.
There were twenty-four hours in each day.
He was invincible.
He knew how to work, and the citadels would go down before him.
She did not think much of his plan.
Not that I know anything about it, of course.
I only bring common judgment to bear.
This education is indispensable for whatever career you select, and it must not be slipshod or sketchy.
You should go to high school.
""I would have to,"" he said grimly.
I must live and buy books and clothes, you know.
This change in him was her handiwork, and she was proud of it and fired with ambition further to help him.
But the most radical change of all, and the one that pleased her most, was the change in his speech.
Not only did he speak more correctly, but he spoke more easily, and there were many new words in his vocabulary.
Also, there was an awkward hesitancy, at times, as he essayed the new words he had learned.
He was just beginning to orientate himself and to feel that he was not wholly an intruder.
He told her of what he had been doing, and of his plan to write for a livelihood and of going on with his studies.
But he was disappointed at her lack of approval.
Up to then he had accepted existence, as he had lived it with all about him, as a good thing.
Who could tell?
He did not know enough to ask the man at the desk, and began his adventures in the philosophy alcove.
He had heard of book philosophy, but had not imagined there had been so much written about it.
The high, bulging shelves of heavy tomes humbled him and at the same time stimulated him.
Here was work for the vigour of his brain.
He could read English, but he saw there an alien speech.
How could she, living the refined life she did?
Higginbotham, who would have preferred the money taking the form of board.
On another night, his vigil was rewarded by a glimpse of Ruth through a second-story window.
He saw only her head and shoulders, and her arms raised as she fixed her hair before a mirror.
Then she pulled down the shade.
She was of the class that dealt with banks.
In one way, he had undergone a moral revolution.
He had always been easy-going.
It was not in his nature to give rebuff.
In the old days he would have smiled back, and gone further and encouraged smiling.
But now it was different.
He did smile back, then looked away, and looked no more deliberately.
But several times, forgetting the existence of the two girls, his eyes caught their smiles.
It was nothing new to him.
But it was different now.
He had it in his heart to wish that they could possess, in some small measure, her goodness and glory.
And not for the world could he hurt them because of their outreaching.
He was not flattered by it; he even felt a slight shame at his lowliness that permitted it.
He left his seat before the curtain went down on the last act, intent on seeing her as she passed out.
Their casual edging across the sidewalk to the curb, as they drew near, apprised him of discovery.
They slowed down, and were in the thick of the crown as they came up with him.
One of them brushed against him and apparently for the first time noticed him.
She was a slender, dark girl, with black, defiant eyes.
But they smiled at him, and he smiled back.
Also, she was struck by his face.
It was almost violent, this health of his, and it seemed to rush out of him and at her in waves of force.
And he, in turn, knew again the swimming sensation of bliss when he felt the contact of her hand in greeting.
The difference between them lay in that she was cool and self-possessed while his face flushed to the roots of the hair.
He stumbled with his old awkwardness after her, and his shoulders swung and lurched perilously.
Once they were seated in the living-room, he began to get on easily - more easily by far than he had expected.
She made it easy for him; and the gracious spirit with which she did it made him love her more madly than ever.
She had thought of this often since their first meeting.
She wanted to help him.
The old fascination of his neck was there, and there was sweetness in the thought of laying her hands upon it.
As he gazed at her and listened, his thoughts grew daring.
I was never inside a house like this.
I wanted it.
I want it now.
Here it is.
I want to make my way to the kind of life you have in this house.
Mebbe I ought to ask him.
Ruth did not speak immediately.
She had never looked in eyes that expressed greater power.
And for that matter so complex and quick was her own mind that she did not have a just appreciation of simplicity.
And yet she had caught an impression of power in the very groping of this mind.
It had seemed to her like a giant writhing and straining at the bonds that held him down.
Her face was all sympathy when she did speak.
You should go back and finish grammar school, and then go through to high school and university.
""But that takes money,"" he interrupted.
But then you have relatives, somebody who could assist you?
He shook his head.
The oldest died in India.
What it means is that a trust is like an egg, and it is not like an egg.
If you want to break an egg you have to do it from the outside.
The only way to break up a trust is from the inside.
Keep sitting on it until it hatches.
Somewhat to my surprise he acknowledged the corner.
""Once,"" said he.
But we lost out.
""Some unforeseen opposition came up, I suppose,"" I said.
We were self-curbed.
It was a case of auto-suppression.
There was a rift within the loot, as Albert Tennyson says.
That man was the most talented conniver at stratagems I ever saw.
Andy was educated, too, besides having a lot of useful information.
Oh, yes, the mine was all right.
The other half interest must have been worth two or three thousand.
I often wondered who owned that mine.
The town had about 2,000 inhabitants, mostly men.
I figured out that their principal means of existence was in living close to tall chaparral.
Me and Andy put up at a hotel that was built like something between a roof-garden and a sectional bookcase.
It began to rain the day we got there.
As the saying is, Juniper Aquarius was sure turning on the water plugs on Mount Amphibious.
But we could see the townspeople making a triangular procession from one to another all day and half the night.
Everybody seemed to know what to do with as much money as they had.
Bird City was built between the Rio Grande and a deep wide arroyo that used to be the old bed of the river.
Andy looks at it a long time.
And then he unfolds to me an instantaneous idea that has occurred to him.
Right there was organized a trust; and we walked back into town and put it on the market.";
    #endregion
  }
}
