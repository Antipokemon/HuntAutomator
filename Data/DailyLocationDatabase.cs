using System.Numerics;

namespace HuntAutomator.Data;

// Optional fast-path coordinates. Unknown marks fall back to a full map patrol.
// These can be expanded without changing the engine.
internal static class DailyLocationDatabase
{
    private static readonly Dictionary<uint, Vector2> Positions = new()
    {


        // Shadowbringers
        // Lakeland
        [8498] = new(19.0f, 9.0f), // Chiliad Cama
        [8502] = new(28.0f, 23.2f), // Violet Triffid
        [8503] = new(14.0f, 16.5f), // Gnole
        [8504] = new(24.4f, 23.9f), // Wetland Warg
        [8505] = new(33.2f, 10.0f), // White Gremlin
        [8507] = new(25.8f, 23.3f), // Hoptrap
        [8508] = new(28.5f, 36.7f), // Wolverine
        [8511] = new(11.3f, 11.0f), // Smilodon
        [8514] = new(34.2f, 17.0f), // Ya-te-veo
        [8515] = new(29.0f, 17.6f), // Proterosuchus
        [8786] = new(20.5f, 25.3f), // Lake Viper

        // Kholusia
        [8517] = new(31.9f, 18.9f), // Ironbeard
        [8518] = new(36.4f, 28.7f), // Hobgoblin
        [8520] = new(17.0f, 18.0f), // Defective Talos
        [8522] = new(34.8f, 10.5f), // Sulfur Byrgen
        [8523] = new(35.4f, 29.2f), // Maultasche
        [8524] = new(14.3f, 11.4f), // Huldu
        [8525] = new(14.3f, 27.1f), // Island Rail
        [8527] = new(17.0f, 11.0f), // Cliffkite
        [8528] = new(27.1f, 13.8f), // Cliffmole
        [8529] = new(08.0f, 18.0f), // Scree Gnome
        [8532] = new(17.8f, 26.5f), // Wood Eyes
        [8533] = new(25.0f, 23.5f), // Island Wolf
        [8534] = new(10.1f, 29.6f), // Kholusian Bison
        [8536] = new(32.5f, 26.2f), // Whiptail
        [8538] = new(22.5f, 9.6f), // Highland Hyssop
        [8539] = new(19.9f, 33.0f), // Tragopan
        [8540] = new(13.0f, 15.0f), // Saichania
        [8541] = new(21.0f, 8.7f), // Gulgnu
        [8542] = new(21.6f, 32.0f), // Germinant

        // Amh Araeng
        [8544] = new(11.4f, 30.4f), // Masterless Talos
        [8545] = new(19.1f, 20.9f), // Evil Weapon
        [8547] = new(30.4f, 12.3f), // Gigantender
        [8550] = new(29.4f, 25.4f), // Ancient Lizard
        [8556] = new(29.4f, 21.7f), // Sand Mole
        [8557] = new(12.7f, 19.0f), // Thistle Mole
        [8558] = new(30.9f, 27.3f), // Scissorjaws
        [8559] = new(21.5f, 9.7f), // Gnome
        [8561] = new(13.9f, 18.2f), // Debitage
        [8562] = new(27.1f, 29.6f), // Ghilman
        [8563] = new(25.0f, 34.3f), // Flame Zonure
        [8565] = new(15.2f, 16.7f), // Phorusrhacos
        [8566] = new(21.7f, 9.8f), // Desert Coyote
        [8567] = new(23.9f, 31.8f), // Molamander

        // Il Mheg
        [8155] = new(08.4f, 30.0f), // Flower Basket
        [8569] = new(18.0f, 31.0f), // Echevore
        [8574] = new(31.0f, 14.3f), // Garden Porxie
        [8575] = new(19.9f, 16.3f), // Phooka
        [8576] = new(11.1f, 26.0f), // Etainmoth
        [8577] = new(29.4f, 12.7f), // Green Glider
        [8578] = new(21.0f, 8.8f), // Moss Fungus
        [8581] = new(07.8f, 18.7f), // Hawker
        [8582] = new(25.0f, 11.0f), // Rainbow Lorikeet
        [8583] = new(29.5f, 11.4f), // Tot Aevis
        [8584] = new(30.4f, 10.6f), // Rabbit's Tail
        [8585] = new(19.0f, 32.0f), // Rosebear
        [8586] = new(31.6f, 6.4f), // Garden Crocota
        [8587] = new(32.0f, 5.8f), // Werewood
        [8590] = new(09.4f, 15.0f), // Killer Bee

        // The Rak'tika Greatwood
        [8596] = new(08.8f, 35.6f), // Tomatl
        [8597] = new(27.3f, 25.6f), // Forest Echo
        [8598] = new(25.1f, 14.2f), // Cracked Ronkan Doll
        [8599] = new(23.0f, 14.0f), // Cracked Ronkan Thorn
        [8600] = new(16.0f, 32.0f), // Vampire Vine
        [8601] = new(23.4f, 7.6f), // Greatwood Rail
        [8603] = new(29.4f, 21.7f), // Snapweed
        [8604] = new(12.0f, 34.0f), // Atrociraptor
        [8606] = new(27.7f, 23.2f), // Gizamaluk
        [8609] = new(16.9f, 33.3f), // Helm Beetle
        [8610] = new(34.1f, 16.5f), // Floor Mandrill
        [8611] = new(15.0f, 19.4f), // Wild Swine
        [8612] = new(24.9f, 30.2f), // Caracal
        [8614] = new(25.0f, 7.2f), // Woodbat
        [8616] = new(27.9f, 21.2f), // Tarichuk
        [8789] = new(21.1f, 13.2f), // Cracked Ronkan Vessel

        // The Tempest
        [8618] = new(28.6f, 6.2f), // Clinoid
        [8619] = new(28.2f, 18.3f), // Dagon
        [8621] = new(22.6f, 31.7f), // Cubus
        [8622] = new(25.1f, 18.6f), // Sea Anemone
        [8623] = new(32.1f, 11.7f), // Amphisbaena
        [8625] = new(32.5f, 21.5f), // Morgawr
        [8626] = new(36.6f, 16.6f), // Trilobite
        [8629] = new(27.7f, 8.7f), // Sea Gelatin
        [8630] = new(29.0f, 21.0f), // Tempest Swallow
        [8631] = new(35.8f, 7.2f), // Blue Swimmer

        // Endwalker
        // Labyrinthos
        [10668] = new(28.8f, 8.8f), // Troll
        [10669] = new(31.0f, 25.5f), // Genomos
        [10670] = new(15.0f, 6.5f), // Caribou
        [10672] = new(32.0f, 8.8f), // Limascabra
        [10673] = new(21.5f, 13.5f), // Luncheon Toad
        [10674] = new(17.0f, 12.0f), // Yakow
        [10677] = new(34.0f, 15.0f), // Labyrinth Screamer
        [10678] = new(24.0f, 10.7f), // Northern Snapweed
        [10679] = new(26.0f, 14.5f), // Pephredo
        [10683] = new(37.5f, 19.5f), // Mythrilcap

        // Thavnair
        [10697] = new(19.0f, 23.9f), // Pisaca
        [10698] = new(13.8f, 18.5f), // Vajralangula
        [10699] = new(19.2f, 32.6f), // Kacchapa
        [10700] = new(18.4f, 26.7f), // Hamsa
        [10701] = new(29.1f, 12.2f), // Asvattha
        [10702] = new(27.1f, 27.8f), // Guhasaya
        [10703] = new(27.0f, 17.4f), // Bhujamga
        [10704] = new(17.6f, 17.8f), // Sotormurg
        [10705] = new(22.7f, 30.4f), // Gaja
        [10706] = new(19.1f, 11.7f), // Thavnairian Jhammel
        [10707] = new(25.9f, 19.0f), // Ufiti
        [10709] = new(09.2f, 12.8f), // Chamrosh
        [10711] = new(16.1f, 9.2f), // Starmite
        [10712] = new(14.3f, 12.7f), // Manjusaka
        [10713] = new(23.3f, 19.9f), // Odqan
        [10715] = new(13.4f, 28.5f), // Akyaali Crab
        [10716] = new(08.2f, 16.2f), // Valras

        // Garlemald
        [10648] = new(18.8f, 9.8f), // Automated Satellite
        [10649] = new(25.5f, 17.5f), // Automated Death Machine
        [10650] = new(15.5f, 19.5f), // Automated Cavalry
        [10651] = new(21.8f, 17.4f), // Automated Bit
        [10652] = new(15.7f, 9.8f), // Automated Roader
        [10653] = new(29.5f, 13.7f), // Automated Slasher
        [10654] = new(24.3f, 14.9f), // Automated Colossus
        [10655] = new(12.9f, 11.7f), // Automated Avenger
        [10656] = new(29.6f, 30.3f), // Almasty
        [10657] = new(14.6f, 26.1f), // Eblan Bear
        [10658] = new(31.3f, 17.4f), // Eblan Icetrap
        [10659] = new(19.8f, 29.1f), // Ovibos
        [10660] = new(22.3f, 24.9f), // Jotunn
        [10661] = new(28.4f, 33.0f), // Ceruleum Zoblyn
        [10662] = new(25.4f, 31.5f), // Ilsabardian Tursus
        [10663] = new(18.7f, 24.8f), // Canis Lupinus
        [10664] = new(26.1f, 26.5f), // Overgrown Rose

        // Mare Lamentorum
        [10458] = new(23.9f, 20.0f), // Daphnia
        [10459] = new(23.7f, 20.3f), // Osculator
        [10460] = new(08.6f, 35.5f), // Sweeper
        [10461] = new(27.3f, 26.0f), // Wanderer
        [10462] = new(31.1f, 32.2f), // Weeper
        [10463] = new(19.8f, 22.5f), // Thinker
        [10464] = new(26.0f, 34.0f), // Regolith
        [10465] = new(21.4f, 32.2f), // Trimmer
        [10467] = new(12.0f, 36.7f), // Panopt
        [10468] = new(11.5f, 22.3f), // Dynamite
        [10469] = new(16.7f, 31.8f), // Armalcolite
        [10470] = new(12.9f, 9.6f), // Caretaker
        [10471] = new(16.1f, 24.9f), // Mousse
        [10473] = new(31.2f, 27.0f), // Downfall Alarum
        [10474] = new(33.6f, 26.2f), // Downfall Droid
        [10475] = new(34.5f, 28.0f), // Downfall Hunter
        [10476] = new(13.0f, 10.0f), // Supporter
        [10477] = new(30.1f, 11.0f), // Scraper

        // Elpis
        [10590] = new(25.7f, 33.9f), // Ophion
        [10591] = new(16.5f, 29.9f), // Yggdreant
        [10592] = new(22.6f, 20.0f), // Okyupete
        [10594] = new(12.4f, 31.8f), // Gryps
        [10595] = new(26.6f, 29.7f), // Monoceros
        [10596] = new(10.1f, 14.1f), // Pegasos
        [10597] = new(28.7f, 25.6f), // Bird of Elpis
        [10599] = new(33.4f, 14.3f), // Hippe
        [10600] = new(14.1f, 9.9f), // Harpuia
        [10601] = new(25.0f, 10.0f), // Morbol Marquis
        [10602] = new(29.2f, 9.3f), // Akantha
        [10603] = new(24.4f, 14.3f), // Lykopersikon
        [10606] = new(21.5f, 6.3f), // Lotis
        [10607] = new(10.2f, 34.6f), // Phanopsyche
        [10608] = new(12.9f, 23.4f), // Melanion
        [10609] = new(12.9f, 8.7f), // Ophiotauros
        [10610] = new(13.3f, 15.7f), // Elpis Minotaur
        [10611] = new(30.7f, 17.1f), // Remora

        // Ultima Thule
        [10419] = new(30.1f, 25.9f), // Broken Omicron
        [10420] = new(19.3f, 11.8f), // Drifting Ea
        [10421] = new(34.8f, 28.8f), // Beta
        [10422] = new(32.9f, 28.8f), // Delta
        [10423] = new(36.5f, 25.9f), // Lambda
        [10424] = new(32.1f, 26.6f), // Level Tricker
        [10427] = new(10.0f, 30.0f), // Stellar Amphiptere
        [10430] = new(14.4f, 28.2f), // Stellar Brobinyak
        [10435] = new(16.3f, 14.1f), // Other One

        // Dawntrail
        // Urqopacha
        [13079] = new(32.0f, 13.4f), // Alpaca
        [13090] = new(22.5f, 16.9f), // Bandercoeurl
        [13083] = new(22.5f, 11.8f), // Barbmole
        [13081] = new(33.5f, 34.2f), // Bloodsucker
        [13087] = new(28.7f, 9.1f), // Chaba Gedan
        [13084] = new(22.4f, 33.9f), // Chirwagur Sabreur
        [13085] = new(16.6f, 28.0f), // Flint
        [13094] = new(15.9f, 23.7f), // Huallepen
        [13096] = new(25.3f, 22.2f), // Longjaw
        [13080] = new(19.5f, 14.8f), // Megamaguey
        [13095] = new(35.0f, 27.5f), // Molten Phoebad
        [13091] = new(19.3f, 17.1f), // Mountain Bear
        [13093] = new(24.2f, 27.3f), // Naryordor
        [13092] = new(15.2f, 13.3f), // Notocactuar
        [13097] = new(09.4f, 22.9f), // Ridgetrap
        [13088] = new(25.7f, 17.0f), // Siehnam
        [13082] = new(25.9f, 14.0f), // Silver Lobo
        [13086] = new(31.9f, 18.5f), // Tulichu
        [13098] = new(28.1f, 28.4f), // Tulidile
        [13089] = new(30.8f, 15.4f), // Turali Ratel

        // Kozama'uka
        [12946] = new(19.5f, 23.8f), // Bird of Ligaka
        [12935] = new(14.0f, 19.3f), // Glowfly
        [12930] = new(10.2f, 9.5f), // Hammerhead Crocodile
        [12936] = new(21.1f, 12.7f), // Heavy Matamata
        [12952] = new(13.2f, 29.6f), // Jungle Iguana
        [12934] = new(14.2f, 16.3f), // Jungle Orobon
        [12938] = new(13.8f, 11.2f), // Jungle Pelican
        [12943] = new(11.2f, 22.8f), // Lesser Apollyon
        [12941] = new(33.0f, 14.6f), // Ocelot
        [12949] = new(36.9f, 34.9f), // Paper Wasp
        [12939] = new(29.9f, 15.5f), // Poison Frog
        [12933] = new(15.7f, 14.4f), // Rhino Beetle
        [12937] = new(26.8f, 12.3f), // Stinkshell
        [12944] = new(19.9f, 28.4f), // Swampmonk
        [12948] = new(34.2f, 27.5f), // Tegu
        [12947] = new(08.0f, 32.5f), // Tomaton
        [12932] = new(20.7f, 15.8f), // Toucalibri
        [12950] = new(17.9f, 32.2f), // Turali Morbol
        [12951] = new(08.7f, 26.7f), // Turali Netzach
        [12942] = new(31.5f, 19.5f), // U'out
        [12931] = new(14.8f, 5.5f), // Uolon
        [12945] = new(28.9f, 25.3f), // Widowmaker
        [12940] = new(33.6f, 8.5f), // Woodsman

        // Yak T'el
        [12957] = new(21.1f, 5.6f), // Balyaborr
        [12966] = new(07.4f, 24.4f), // Blue Leafkin
        [12969] = new(16.4f, 30.9f), // Blue Morpho
        [12964] = new(17.8f, 24.4f), // Branchbearer
        [12971] = new(16.3f, 37.7f), // Fly Agaric
        [12970] = new(20.4f, 18.1f), // Ja Tiika Moth
        [12958] = new(24.3f, 6.3f), // Killer Piranha
        [12955] = new(09.5f, 20.4f), // Leaf Mantis
        [12965] = new(30.2f, 28.3f), // Mourner
        [12954] = new(17.0f, 13.8f), // Necrosis
        [12962] = new(35.3f, 23.1f), // Pitcher Weed
        [12967] = new(30.6f, 35.7f), // Sarracenia
        [12960] = new(28.0f, 18.4f), // T'ohsoq
        [12961] = new(32.8f, 20.5f), // T'ohts'on
        [12953] = new(24.0f, 11.8f), // Ty'aitya
        [12956] = new(12.4f, 9.9f), // Vawtsaral Br'aax
        [12959] = new(32.2f, 12.7f), // Yak T'el Squib

        // Shaaloani
        [12990] = new(14.7f, 9.4f), // Aspis
        [12989] = new(27.6f, 13.1f), // Ceratoraptor
        [12975] = new(11.4f, 17.1f), // Cerule Anala
        [12977] = new(19.9f, 21.5f), // Cerule Bomb
        [12992] = new(31.8f, 23.8f), // Flying Popoto
        [12996] = new(28.8f, 23.5f), // Grasslands Worm
        [12995] = new(24.4f, 15.6f), // Gritclaw
        [12997] = new(12.3f, 12.9f), // Horned Lizard
        [12988] = new(21.4f, 31.8f), // Lunyucaua'pya
        [12987] = new(18.0f, 31.6f), // Lunyuhiyshahe
        [12991] = new(24.3f, 11.3f), // Rroneek
        [12976] = new(25.9f, 29.0f), // Sunbeard
        [12993] = new(35.1f, 11.1f), // Toari Alligator
        [12972] = new(30.3f, 33.1f), // Tumbleclaw
        [12994] = new(29.2f, 7.9f), // Turali Hawksbill
        [12978] = new(14.1f, 23.3f), // Wild Dhara
        [12986] = new(12.7f, 30.2f), // Yeheheceyaa

        // Heritage Found
        [13115] = new(09.6f, 19.5f), // Asterodia
        [13101] = new(33.4f, 27.7f), // Axe Beak
        [13103] = new(22.5f, 16.7f), // Bolt Hound
        [13116] = new(28.2f, 26.7f), // Cauahealoa
        [13117] = new(32.6f, 22.7f), // Cauahepya
        [13108] = new(15.8f, 22.2f), // Defective Aerostat
        [13107] = new(15.8f, 22.2f), // Defective Sentry R8
        [13106] = new(10.6f, 26.2f), // Defective Sentry S8
        [13109] = new(11.3f, 11.5f), // Defective Turret
        [13105] = new(21.3f, 27.6f), // Eyeclops
        [13104] = new(14.8f, 17.2f), // Gomphotherium
        [13113] = new(35.2f, 14.2f), // Katoblepas
        [13112] = new(24.3f, 20.9f), // Myrmeleon
        [13114] = new(11.2f, 33.2f), // Python
        [13111] = new(30.8f, 13.9f), // Thunder Spirit
        [13110] = new(15.6f, 32.4f), // Woolback
        [13102] = new(24.8f, 7.3f), // Yyenisheyni Bat

        // Living Memory
        [13121] = new(33.1f, 34.4f), // Acrocat
        [13137] = new(12.0f, 18.7f), // Agavoides
        [13133] = new(30.5f, 17.1f), // Alexandrian Clipper
        [13130] = new(26.9f, 17.6f), // Blazing Soul
        [13127] = new(35.9f, 27.2f), // Brownie
        [13136] = new(17.8f, 21.9f), // Everlasting Yew
        [13124] = new(09.9f, 36.2f), // Fluid Soul
        [13131] = new(36.5f, 18.3f), // Gargantua
        [13120] = new(32.3f, 27.1f), // Gemkeeper
        [13129] = new(26.6f, 8.7f), // Matchlock Scorpion
        [13118] = new(33.2f, 16.3f), // Outrunner
        [13132] = new(30.3f, 12.5f), // Pineapple
        [13125] = new(13.0f, 34.5f), // Remembird
        [13119] = new(28.4f, 31.6f), // Seeker Bat
        [13139] = new(18.0f, 15.2f), // Timberman
        [13122] = new(17.7f, 30.5f), // Torbalan
        [13138] = new(11.8f, 12.9f), // Walking Tree
    };

    // Extra community-known spawn clusters. The first entry in Positions remains the
    // preferred fast path; these are searched before falling back to a full-map patrol.
    private static readonly Dictionary<uint, Vector2[]> AdditionalPositions = new()
    {
        // Alpaca groups in Urqopacha. Hunt Buddy lists more than one cluster, and the
        // old (32.0, 13.4) point can leave navigation at the edge of the northern group.
        [13079] = [new(32.0f, 14.9f), new(12.5f, 8.8f)],
    };

    public static IReadOnlyList<Vector2> GetPositions(uint mobId)
    {
        if (!Positions.TryGetValue(mobId, out var primary))
            return Array.Empty<Vector2>();

        if (!AdditionalPositions.TryGetValue(mobId, out var additional))
            return new[] { primary };

        var result = new Vector2[additional.Length + 1];
        result[0] = primary;
        additional.CopyTo(result, 1);
        return result;
    }
}
