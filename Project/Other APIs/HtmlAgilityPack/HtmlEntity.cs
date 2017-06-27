namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Class HtmlEntity.
    /// </summary>
    public class HtmlEntity
    {
        private static Dictionary<int, string> _entityName = new Dictionary<int, string>();
        private static Dictionary<string, int> _entityValue = new Dictionary<string, int>();
        private static readonly int _maxEntitySize;

        static HtmlEntity()
        {
            _entityValue.Add("nbsp", 160);
            _entityName.Add(160, "nbsp");
            _entityValue.Add("iexcl", 0xa1);
            _entityName.Add(0xa1, "iexcl");
            _entityValue.Add("cent", 0xa2);
            _entityName.Add(0xa2, "cent");
            _entityValue.Add("pound", 0xa3);
            _entityName.Add(0xa3, "pound");
            _entityValue.Add("curren", 0xa4);
            _entityName.Add(0xa4, "curren");
            _entityValue.Add("yen", 0xa5);
            _entityName.Add(0xa5, "yen");
            _entityValue.Add("brvbar", 0xa6);
            _entityName.Add(0xa6, "brvbar");
            _entityValue.Add("sect", 0xa7);
            _entityName.Add(0xa7, "sect");
            _entityValue.Add("uml", 0xa8);
            _entityName.Add(0xa8, "uml");
            _entityValue.Add("copy", 0xa9);
            _entityName.Add(0xa9, "copy");
            _entityValue.Add("ordf", 170);
            _entityName.Add(170, "ordf");
            _entityValue.Add("laquo", 0xab);
            _entityName.Add(0xab, "laquo");
            _entityValue.Add("not", 0xac);
            _entityName.Add(0xac, "not");
            _entityValue.Add("shy", 0xad);
            _entityName.Add(0xad, "shy");
            _entityValue.Add("reg", 0xae);
            _entityName.Add(0xae, "reg");
            _entityValue.Add("macr", 0xaf);
            _entityName.Add(0xaf, "macr");
            _entityValue.Add("deg", 0xb0);
            _entityName.Add(0xb0, "deg");
            _entityValue.Add("plusmn", 0xb1);
            _entityName.Add(0xb1, "plusmn");
            _entityValue.Add("sup2", 0xb2);
            _entityName.Add(0xb2, "sup2");
            _entityValue.Add("sup3", 0xb3);
            _entityName.Add(0xb3, "sup3");
            _entityValue.Add("acute", 180);
            _entityName.Add(180, "acute");
            _entityValue.Add("micro", 0xb5);
            _entityName.Add(0xb5, "micro");
            _entityValue.Add("para", 0xb6);
            _entityName.Add(0xb6, "para");
            _entityValue.Add("middot", 0xb7);
            _entityName.Add(0xb7, "middot");
            _entityValue.Add("cedil", 0xb8);
            _entityName.Add(0xb8, "cedil");
            _entityValue.Add("sup1", 0xb9);
            _entityName.Add(0xb9, "sup1");
            _entityValue.Add("ordm", 0xba);
            _entityName.Add(0xba, "ordm");
            _entityValue.Add("raquo", 0xbb);
            _entityName.Add(0xbb, "raquo");
            _entityValue.Add("frac14", 0xbc);
            _entityName.Add(0xbc, "frac14");
            _entityValue.Add("frac12", 0xbd);
            _entityName.Add(0xbd, "frac12");
            _entityValue.Add("frac34", 190);
            _entityName.Add(190, "frac34");
            _entityValue.Add("iquest", 0xbf);
            _entityName.Add(0xbf, "iquest");
            _entityValue.Add("Agrave", 0xc0);
            _entityName.Add(0xc0, "Agrave");
            _entityValue.Add("Aacute", 0xc1);
            _entityName.Add(0xc1, "Aacute");
            _entityValue.Add("Acirc", 0xc2);
            _entityName.Add(0xc2, "Acirc");
            _entityValue.Add("Atilde", 0xc3);
            _entityName.Add(0xc3, "Atilde");
            _entityValue.Add("Auml", 0xc4);
            _entityName.Add(0xc4, "Auml");
            _entityValue.Add("Aring", 0xc5);
            _entityName.Add(0xc5, "Aring");
            _entityValue.Add("AElig", 0xc6);
            _entityName.Add(0xc6, "AElig");
            _entityValue.Add("Ccedil", 0xc7);
            _entityName.Add(0xc7, "Ccedil");
            _entityValue.Add("Egrave", 200);
            _entityName.Add(200, "Egrave");
            _entityValue.Add("Eacute", 0xc9);
            _entityName.Add(0xc9, "Eacute");
            _entityValue.Add("Ecirc", 0xca);
            _entityName.Add(0xca, "Ecirc");
            _entityValue.Add("Euml", 0xcb);
            _entityName.Add(0xcb, "Euml");
            _entityValue.Add("Igrave", 0xcc);
            _entityName.Add(0xcc, "Igrave");
            _entityValue.Add("Iacute", 0xcd);
            _entityName.Add(0xcd, "Iacute");
            _entityValue.Add("Icirc", 0xce);
            _entityName.Add(0xce, "Icirc");
            _entityValue.Add("Iuml", 0xcf);
            _entityName.Add(0xcf, "Iuml");
            _entityValue.Add("ETH", 0xd0);
            _entityName.Add(0xd0, "ETH");
            _entityValue.Add("Ntilde", 0xd1);
            _entityName.Add(0xd1, "Ntilde");
            _entityValue.Add("Ograve", 210);
            _entityName.Add(210, "Ograve");
            _entityValue.Add("Oacute", 0xd3);
            _entityName.Add(0xd3, "Oacute");
            _entityValue.Add("Ocirc", 0xd4);
            _entityName.Add(0xd4, "Ocirc");
            _entityValue.Add("Otilde", 0xd5);
            _entityName.Add(0xd5, "Otilde");
            _entityValue.Add("Ouml", 0xd6);
            _entityName.Add(0xd6, "Ouml");
            _entityValue.Add("times", 0xd7);
            _entityName.Add(0xd7, "times");
            _entityValue.Add("Oslash", 0xd8);
            _entityName.Add(0xd8, "Oslash");
            _entityValue.Add("Ugrave", 0xd9);
            _entityName.Add(0xd9, "Ugrave");
            _entityValue.Add("Uacute", 0xda);
            _entityName.Add(0xda, "Uacute");
            _entityValue.Add("Ucirc", 0xdb);
            _entityName.Add(0xdb, "Ucirc");
            _entityValue.Add("Uuml", 220);
            _entityName.Add(220, "Uuml");
            _entityValue.Add("Yacute", 0xdd);
            _entityName.Add(0xdd, "Yacute");
            _entityValue.Add("THORN", 0xde);
            _entityName.Add(0xde, "THORN");
            _entityValue.Add("szlig", 0xdf);
            _entityName.Add(0xdf, "szlig");
            _entityValue.Add("agrave", 0xe0);
            _entityName.Add(0xe0, "agrave");
            _entityValue.Add("aacute", 0xe1);
            _entityName.Add(0xe1, "aacute");
            _entityValue.Add("acirc", 0xe2);
            _entityName.Add(0xe2, "acirc");
            _entityValue.Add("atilde", 0xe3);
            _entityName.Add(0xe3, "atilde");
            _entityValue.Add("auml", 0xe4);
            _entityName.Add(0xe4, "auml");
            _entityValue.Add("aring", 0xe5);
            _entityName.Add(0xe5, "aring");
            _entityValue.Add("aelig", 230);
            _entityName.Add(230, "aelig");
            _entityValue.Add("ccedil", 0xe7);
            _entityName.Add(0xe7, "ccedil");
            _entityValue.Add("egrave", 0xe8);
            _entityName.Add(0xe8, "egrave");
            _entityValue.Add("eacute", 0xe9);
            _entityName.Add(0xe9, "eacute");
            _entityValue.Add("ecirc", 0xea);
            _entityName.Add(0xea, "ecirc");
            _entityValue.Add("euml", 0xeb);
            _entityName.Add(0xeb, "euml");
            _entityValue.Add("igrave", 0xec);
            _entityName.Add(0xec, "igrave");
            _entityValue.Add("iacute", 0xed);
            _entityName.Add(0xed, "iacute");
            _entityValue.Add("icirc", 0xee);
            _entityName.Add(0xee, "icirc");
            _entityValue.Add("iuml", 0xef);
            _entityName.Add(0xef, "iuml");
            _entityValue.Add("eth", 240);
            _entityName.Add(240, "eth");
            _entityValue.Add("ntilde", 0xf1);
            _entityName.Add(0xf1, "ntilde");
            _entityValue.Add("ograve", 0xf2);
            _entityName.Add(0xf2, "ograve");
            _entityValue.Add("oacute", 0xf3);
            _entityName.Add(0xf3, "oacute");
            _entityValue.Add("ocirc", 0xf4);
            _entityName.Add(0xf4, "ocirc");
            _entityValue.Add("otilde", 0xf5);
            _entityName.Add(0xf5, "otilde");
            _entityValue.Add("ouml", 0xf6);
            _entityName.Add(0xf6, "ouml");
            _entityValue.Add("divide", 0xf7);
            _entityName.Add(0xf7, "divide");
            _entityValue.Add("oslash", 0xf8);
            _entityName.Add(0xf8, "oslash");
            _entityValue.Add("ugrave", 0xf9);
            _entityName.Add(0xf9, "ugrave");
            _entityValue.Add("uacute", 250);
            _entityName.Add(250, "uacute");
            _entityValue.Add("ucirc", 0xfb);
            _entityName.Add(0xfb, "ucirc");
            _entityValue.Add("uuml", 0xfc);
            _entityName.Add(0xfc, "uuml");
            _entityValue.Add("yacute", 0xfd);
            _entityName.Add(0xfd, "yacute");
            _entityValue.Add("thorn", 0xfe);
            _entityName.Add(0xfe, "thorn");
            _entityValue.Add("yuml", 0xff);
            _entityName.Add(0xff, "yuml");
            _entityValue.Add("fnof", 0x192);
            _entityName.Add(0x192, "fnof");
            _entityValue.Add("Alpha", 0x391);
            _entityName.Add(0x391, "Alpha");
            _entityValue.Add("Beta", 0x392);
            _entityName.Add(0x392, "Beta");
            _entityValue.Add("Gamma", 0x393);
            _entityName.Add(0x393, "Gamma");
            _entityValue.Add("Delta", 0x394);
            _entityName.Add(0x394, "Delta");
            _entityValue.Add("Epsilon", 0x395);
            _entityName.Add(0x395, "Epsilon");
            _entityValue.Add("Zeta", 0x396);
            _entityName.Add(0x396, "Zeta");
            _entityValue.Add("Eta", 0x397);
            _entityName.Add(0x397, "Eta");
            _entityValue.Add("Theta", 920);
            _entityName.Add(920, "Theta");
            _entityValue.Add("Iota", 0x399);
            _entityName.Add(0x399, "Iota");
            _entityValue.Add("Kappa", 0x39a);
            _entityName.Add(0x39a, "Kappa");
            _entityValue.Add("Lambda", 0x39b);
            _entityName.Add(0x39b, "Lambda");
            _entityValue.Add("Mu", 0x39c);
            _entityName.Add(0x39c, "Mu");
            _entityValue.Add("Nu", 0x39d);
            _entityName.Add(0x39d, "Nu");
            _entityValue.Add("Xi", 0x39e);
            _entityName.Add(0x39e, "Xi");
            _entityValue.Add("Omicron", 0x39f);
            _entityName.Add(0x39f, "Omicron");
            _entityValue.Add("Pi", 0x3a0);
            _entityName.Add(0x3a0, "Pi");
            _entityValue.Add("Rho", 0x3a1);
            _entityName.Add(0x3a1, "Rho");
            _entityValue.Add("Sigma", 0x3a3);
            _entityName.Add(0x3a3, "Sigma");
            _entityValue.Add("Tau", 0x3a4);
            _entityName.Add(0x3a4, "Tau");
            _entityValue.Add("Upsilon", 0x3a5);
            _entityName.Add(0x3a5, "Upsilon");
            _entityValue.Add("Phi", 0x3a6);
            _entityName.Add(0x3a6, "Phi");
            _entityValue.Add("Chi", 0x3a7);
            _entityName.Add(0x3a7, "Chi");
            _entityValue.Add("Psi", 0x3a8);
            _entityName.Add(0x3a8, "Psi");
            _entityValue.Add("Omega", 0x3a9);
            _entityName.Add(0x3a9, "Omega");
            _entityValue.Add("alpha", 0x3b1);
            _entityName.Add(0x3b1, "alpha");
            _entityValue.Add("beta", 0x3b2);
            _entityName.Add(0x3b2, "beta");
            _entityValue.Add("gamma", 0x3b3);
            _entityName.Add(0x3b3, "gamma");
            _entityValue.Add("delta", 0x3b4);
            _entityName.Add(0x3b4, "delta");
            _entityValue.Add("epsilon", 0x3b5);
            _entityName.Add(0x3b5, "epsilon");
            _entityValue.Add("zeta", 950);
            _entityName.Add(950, "zeta");
            _entityValue.Add("eta", 0x3b7);
            _entityName.Add(0x3b7, "eta");
            _entityValue.Add("theta", 0x3b8);
            _entityName.Add(0x3b8, "theta");
            _entityValue.Add("iota", 0x3b9);
            _entityName.Add(0x3b9, "iota");
            _entityValue.Add("kappa", 0x3ba);
            _entityName.Add(0x3ba, "kappa");
            _entityValue.Add("lambda", 0x3bb);
            _entityName.Add(0x3bb, "lambda");
            _entityValue.Add("mu", 0x3bc);
            _entityName.Add(0x3bc, "mu");
            _entityValue.Add("nu", 0x3bd);
            _entityName.Add(0x3bd, "nu");
            _entityValue.Add("xi", 0x3be);
            _entityName.Add(0x3be, "xi");
            _entityValue.Add("omicron", 0x3bf);
            _entityName.Add(0x3bf, "omicron");
            _entityValue.Add("pi", 960);
            _entityName.Add(960, "pi");
            _entityValue.Add("rho", 0x3c1);
            _entityName.Add(0x3c1, "rho");
            _entityValue.Add("sigmaf", 0x3c2);
            _entityName.Add(0x3c2, "sigmaf");
            _entityValue.Add("sigma", 0x3c3);
            _entityName.Add(0x3c3, "sigma");
            _entityValue.Add("tau", 0x3c4);
            _entityName.Add(0x3c4, "tau");
            _entityValue.Add("upsilon", 0x3c5);
            _entityName.Add(0x3c5, "upsilon");
            _entityValue.Add("phi", 0x3c6);
            _entityName.Add(0x3c6, "phi");
            _entityValue.Add("chi", 0x3c7);
            _entityName.Add(0x3c7, "chi");
            _entityValue.Add("psi", 0x3c8);
            _entityName.Add(0x3c8, "psi");
            _entityValue.Add("omega", 0x3c9);
            _entityName.Add(0x3c9, "omega");
            _entityValue.Add("thetasym", 0x3d1);
            _entityName.Add(0x3d1, "thetasym");
            _entityValue.Add("upsih", 0x3d2);
            _entityName.Add(0x3d2, "upsih");
            _entityValue.Add("piv", 0x3d6);
            _entityName.Add(0x3d6, "piv");
            _entityValue.Add("bull", 0x2022);
            _entityName.Add(0x2022, "bull");
            _entityValue.Add("hellip", 0x2026);
            _entityName.Add(0x2026, "hellip");
            _entityValue.Add("prime", 0x2032);
            _entityName.Add(0x2032, "prime");
            _entityValue.Add("Prime", 0x2033);
            _entityName.Add(0x2033, "Prime");
            _entityValue.Add("oline", 0x203e);
            _entityName.Add(0x203e, "oline");
            _entityValue.Add("frasl", 0x2044);
            _entityName.Add(0x2044, "frasl");
            _entityValue.Add("weierp", 0x2118);
            _entityName.Add(0x2118, "weierp");
            _entityValue.Add("image", 0x2111);
            _entityName.Add(0x2111, "image");
            _entityValue.Add("real", 0x211c);
            _entityName.Add(0x211c, "real");
            _entityValue.Add("trade", 0x2122);
            _entityName.Add(0x2122, "trade");
            _entityValue.Add("alefsym", 0x2135);
            _entityName.Add(0x2135, "alefsym");
            _entityValue.Add("larr", 0x2190);
            _entityName.Add(0x2190, "larr");
            _entityValue.Add("uarr", 0x2191);
            _entityName.Add(0x2191, "uarr");
            _entityValue.Add("rarr", 0x2192);
            _entityName.Add(0x2192, "rarr");
            _entityValue.Add("darr", 0x2193);
            _entityName.Add(0x2193, "darr");
            _entityValue.Add("harr", 0x2194);
            _entityName.Add(0x2194, "harr");
            _entityValue.Add("crarr", 0x21b5);
            _entityName.Add(0x21b5, "crarr");
            _entityValue.Add("lArr", 0x21d0);
            _entityName.Add(0x21d0, "lArr");
            _entityValue.Add("uArr", 0x21d1);
            _entityName.Add(0x21d1, "uArr");
            _entityValue.Add("rArr", 0x21d2);
            _entityName.Add(0x21d2, "rArr");
            _entityValue.Add("dArr", 0x21d3);
            _entityName.Add(0x21d3, "dArr");
            _entityValue.Add("hArr", 0x21d4);
            _entityName.Add(0x21d4, "hArr");
            _entityValue.Add("forall", 0x2200);
            _entityName.Add(0x2200, "forall");
            _entityValue.Add("part", 0x2202);
            _entityName.Add(0x2202, "part");
            _entityValue.Add("exist", 0x2203);
            _entityName.Add(0x2203, "exist");
            _entityValue.Add("empty", 0x2205);
            _entityName.Add(0x2205, "empty");
            _entityValue.Add("nabla", 0x2207);
            _entityName.Add(0x2207, "nabla");
            _entityValue.Add("isin", 0x2208);
            _entityName.Add(0x2208, "isin");
            _entityValue.Add("notin", 0x2209);
            _entityName.Add(0x2209, "notin");
            _entityValue.Add("ni", 0x220b);
            _entityName.Add(0x220b, "ni");
            _entityValue.Add("prod", 0x220f);
            _entityName.Add(0x220f, "prod");
            _entityValue.Add("sum", 0x2211);
            _entityName.Add(0x2211, "sum");
            _entityValue.Add("minus", 0x2212);
            _entityName.Add(0x2212, "minus");
            _entityValue.Add("lowast", 0x2217);
            _entityName.Add(0x2217, "lowast");
            _entityValue.Add("radic", 0x221a);
            _entityName.Add(0x221a, "radic");
            _entityValue.Add("prop", 0x221d);
            _entityName.Add(0x221d, "prop");
            _entityValue.Add("infin", 0x221e);
            _entityName.Add(0x221e, "infin");
            _entityValue.Add("ang", 0x2220);
            _entityName.Add(0x2220, "ang");
            _entityValue.Add("and", 0x2227);
            _entityName.Add(0x2227, "and");
            _entityValue.Add("or", 0x2228);
            _entityName.Add(0x2228, "or");
            _entityValue.Add("cap", 0x2229);
            _entityName.Add(0x2229, "cap");
            _entityValue.Add("cup", 0x222a);
            _entityName.Add(0x222a, "cup");
            _entityValue.Add("int", 0x222b);
            _entityName.Add(0x222b, "int");
            _entityValue.Add("there4", 0x2234);
            _entityName.Add(0x2234, "there4");
            _entityValue.Add("sim", 0x223c);
            _entityName.Add(0x223c, "sim");
            _entityValue.Add("cong", 0x2245);
            _entityName.Add(0x2245, "cong");
            _entityValue.Add("asymp", 0x2248);
            _entityName.Add(0x2248, "asymp");
            _entityValue.Add("ne", 0x2260);
            _entityName.Add(0x2260, "ne");
            _entityValue.Add("equiv", 0x2261);
            _entityName.Add(0x2261, "equiv");
            _entityValue.Add("le", 0x2264);
            _entityName.Add(0x2264, "le");
            _entityValue.Add("ge", 0x2265);
            _entityName.Add(0x2265, "ge");
            _entityValue.Add("sub", 0x2282);
            _entityName.Add(0x2282, "sub");
            _entityValue.Add("sup", 0x2283);
            _entityName.Add(0x2283, "sup");
            _entityValue.Add("nsub", 0x2284);
            _entityName.Add(0x2284, "nsub");
            _entityValue.Add("sube", 0x2286);
            _entityName.Add(0x2286, "sube");
            _entityValue.Add("supe", 0x2287);
            _entityName.Add(0x2287, "supe");
            _entityValue.Add("oplus", 0x2295);
            _entityName.Add(0x2295, "oplus");
            _entityValue.Add("otimes", 0x2297);
            _entityName.Add(0x2297, "otimes");
            _entityValue.Add("perp", 0x22a5);
            _entityName.Add(0x22a5, "perp");
            _entityValue.Add("sdot", 0x22c5);
            _entityName.Add(0x22c5, "sdot");
            _entityValue.Add("lceil", 0x2308);
            _entityName.Add(0x2308, "lceil");
            _entityValue.Add("rceil", 0x2309);
            _entityName.Add(0x2309, "rceil");
            _entityValue.Add("lfloor", 0x230a);
            _entityName.Add(0x230a, "lfloor");
            _entityValue.Add("rfloor", 0x230b);
            _entityName.Add(0x230b, "rfloor");
            _entityValue.Add("lang", 0x2329);
            _entityName.Add(0x2329, "lang");
            _entityValue.Add("rang", 0x232a);
            _entityName.Add(0x232a, "rang");
            _entityValue.Add("loz", 0x25ca);
            _entityName.Add(0x25ca, "loz");
            _entityValue.Add("spades", 0x2660);
            _entityName.Add(0x2660, "spades");
            _entityValue.Add("clubs", 0x2663);
            _entityName.Add(0x2663, "clubs");
            _entityValue.Add("hearts", 0x2665);
            _entityName.Add(0x2665, "hearts");
            _entityValue.Add("diams", 0x2666);
            _entityName.Add(0x2666, "diams");
            _entityValue.Add("quot", 0x22);
            _entityName.Add(0x22, "quot");
            _entityValue.Add("amp", 0x26);
            _entityName.Add(0x26, "amp");
            _entityValue.Add("lt", 60);
            _entityName.Add(60, "lt");
            _entityValue.Add("gt", 0x3e);
            _entityName.Add(0x3e, "gt");
            _entityValue.Add("OElig", 0x152);
            _entityName.Add(0x152, "OElig");
            _entityValue.Add("oelig", 0x153);
            _entityName.Add(0x153, "oelig");
            _entityValue.Add("Scaron", 0x160);
            _entityName.Add(0x160, "Scaron");
            _entityValue.Add("scaron", 0x161);
            _entityName.Add(0x161, "scaron");
            _entityValue.Add("Yuml", 0x178);
            _entityName.Add(0x178, "Yuml");
            _entityValue.Add("circ", 710);
            _entityName.Add(710, "circ");
            _entityValue.Add("tilde", 0x2dc);
            _entityName.Add(0x2dc, "tilde");
            _entityValue.Add("ensp", 0x2002);
            _entityName.Add(0x2002, "ensp");
            _entityValue.Add("emsp", 0x2003);
            _entityName.Add(0x2003, "emsp");
            _entityValue.Add("thinsp", 0x2009);
            _entityName.Add(0x2009, "thinsp");
            _entityValue.Add("zwnj", 0x200c);
            _entityName.Add(0x200c, "zwnj");
            _entityValue.Add("zwj", 0x200d);
            _entityName.Add(0x200d, "zwj");
            _entityValue.Add("lrm", 0x200e);
            _entityName.Add(0x200e, "lrm");
            _entityValue.Add("rlm", 0x200f);
            _entityName.Add(0x200f, "rlm");
            _entityValue.Add("ndash", 0x2013);
            _entityName.Add(0x2013, "ndash");
            _entityValue.Add("mdash", 0x2014);
            _entityName.Add(0x2014, "mdash");
            _entityValue.Add("lsquo", 0x2018);
            _entityName.Add(0x2018, "lsquo");
            _entityValue.Add("rsquo", 0x2019);
            _entityName.Add(0x2019, "rsquo");
            _entityValue.Add("sbquo", 0x201a);
            _entityName.Add(0x201a, "sbquo");
            _entityValue.Add("ldquo", 0x201c);
            _entityName.Add(0x201c, "ldquo");
            _entityValue.Add("rdquo", 0x201d);
            _entityName.Add(0x201d, "rdquo");
            _entityValue.Add("bdquo", 0x201e);
            _entityName.Add(0x201e, "bdquo");
            _entityValue.Add("dagger", 0x2020);
            _entityName.Add(0x2020, "dagger");
            _entityValue.Add("Dagger", 0x2021);
            _entityName.Add(0x2021, "Dagger");
            _entityValue.Add("permil", 0x2030);
            _entityName.Add(0x2030, "permil");
            _entityValue.Add("lsaquo", 0x2039);
            _entityName.Add(0x2039, "lsaquo");
            _entityValue.Add("rsaquo", 0x203a);
            _entityName.Add(0x203a, "rsaquo");
            _entityValue.Add("euro", 0x20ac);
            _entityName.Add(0x20ac, "euro");
            _maxEntitySize = 9;
        }

        private HtmlEntity()
        {
        }

        /// <summary>
        /// Des the entitize.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public static string DeEntitize(string text)
        {
            if (text == null)
            {
                return null;
            }
            if (text.Length == 0)
            {
                return text;
            }
            StringBuilder builder = new StringBuilder(text.Length);
            ParseState entityStart = ParseState.Text;
            StringBuilder builder2 = new StringBuilder(10);
            for (int i = 0; i < text.Length; i++)
            {
                switch (entityStart)
                {
                    case ParseState.Text:
                        {
                            char ch = text[i];
                            if (ch != '&')
                            {
                                break;
                            }
                            entityStart = ParseState.EntityStart;
                            continue;
                        }
                    case ParseState.EntityStart:
                        {
                            char ch2 = text[i];
                            if (ch2 == '&')
                            {
                                goto Label_017C;
                            }
                            if (ch2 != ';')
                            {
                                goto Label_019E;
                            }
                            if (builder2.Length != 0)
                            {
                                goto Label_00A0;
                            }
                            builder.Append("&;");
                            goto Label_0178;
                        }
                    default:
                        {
                            continue;
                        }
                }
                builder.Append(text[i]);
                continue;
                Label_00A0:
                if (builder2[0] == '#')
                {
                    string str = builder2.ToString();
                    try
                    {
                        int num2;
                        string str2 = str.Substring(1).Trim().ToLower();
                        if (str2.StartsWith("x"))
                        {
                            num2 = 0x10;
                            str2 = str2.Substring(1);
                        }
                        else
                        {
                            num2 = 10;
                        }
                        int num3 = Convert.ToInt32(str2, num2);
                        builder.Append(Convert.ToChar(num3));
                    }
                    catch
                    {
                        builder.Append("&#" + str + ";");
                    }
                }
                else
                {
                    object obj2 = _entityValue[builder2.ToString()];
                    if (obj2 == null)
                    {
                        builder.Append("&" + builder2 + ";");
                    }
                    else
                    {
                        int num4 = (int)obj2;
                        builder.Append(Convert.ToChar(num4));
                    }
                }
                builder2.Remove(0, builder2.Length);
                Label_0178:
                entityStart = ParseState.Text;
                continue;
                Label_017C:
                builder.Append("&" + builder2);
                builder2.Remove(0, builder2.Length);
                continue;
                Label_019E:
                builder2.Append(text[i]);
                if (builder2.Length > _maxEntitySize)
                {
                    entityStart = ParseState.Text;
                    builder.Append("&" + builder2);
                    builder2.Remove(0, builder2.Length);
                }
            }
            if (entityStart == ParseState.EntityStart)
            {
                builder.Append("&" + builder2);
            }
            return builder.ToString();
        }

        private static void Entitize(HtmlAttributeCollection collection)
        {
            foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)collection)
            {
                attribute.Value = Entitize(attribute.Value);
            }
        }

        /// <summary>
        /// Entitizes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">node</exception>
        public static HtmlNode Entitize(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            HtmlNode node2 = node.CloneNode(true);
            if (node2.HasAttributes)
            {
                Entitize(node2.Attributes);
            }
            if (node2.HasChildNodes)
            {
                Entitize(node2.ChildNodes);
                return node2;
            }
            if (node2.NodeType == HtmlNodeType.Text)
            {
                ((HtmlTextNode)node2).Text = Entitize(((HtmlTextNode)node2).Text, true, true);
            }
            return node2;
        }

        private static void Entitize(HtmlNodeCollection collection)
        {
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)collection)
            {
                if (node.HasAttributes)
                {
                    Entitize(node.Attributes);
                }
                if (node.HasChildNodes)
                {
                    Entitize(node.ChildNodes);
                }
                else if (node.NodeType == HtmlNodeType.Text)
                {
                    ((HtmlTextNode)node).Text = Entitize(((HtmlTextNode)node).Text, true, true);
                }
            }
        }

        /// <summary>
        /// Entitizes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        public static string Entitize(string text)
        {
            return Entitize(text, true);
        }

        /// <summary>
        /// Entitizes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="useNames">if set to <c>true</c> [use names].</param>
        /// <returns>System.String.</returns>
        public static string Entitize(string text, bool useNames)
        {
            return Entitize(text, useNames, false);
        }

        /// <summary>
        /// Entitizes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="useNames">if set to <c>true</c> [use names].</param>
        /// <param name="entitizeQuotAmpAndLtGt">if set to <c>true</c> [entitize quot amp and lt gt].</param>
        /// <returns>System.String.</returns>
        public static string Entitize(string text, bool useNames, bool entitizeQuotAmpAndLtGt)
        {
            if (text == null)
            {
                return null;
            }
            if (text.Length == 0)
            {
                return text;
            }
            StringBuilder builder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                int num2 = text[i];
                if ((num2 > 0x7f) || (entitizeQuotAmpAndLtGt && (((num2 == 0x22) || (num2 == 0x26)) || ((num2 == 60) || (num2 == 0x3e)))))
                {
                    string str = _entityName[num2];
                    if ((str == null) || !useNames)
                    {
                        builder.Append("&#" + num2 + ";");
                    }
                    else
                    {
                        builder.Append("&" + str + ";");
                    }
                }
                else
                {
                    builder.Append(text[i]);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public static Dictionary<int, string> EntityName
        {
            get
            {
                return _entityName;
            }
        }

        /// <summary>
        /// Gets the entity value.
        /// </summary>
        /// <value>The entity value.</value>
        public static Dictionary<string, int> EntityValue
        {
            get
            {
                return _entityValue;
            }
        }

        private enum ParseState
        {
            /// <summary>
            /// The text
            /// </summary>
            Text,

            /// <summary>
            /// The entity start
            /// </summary>
            EntityStart
        }
    }
}