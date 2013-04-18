if(typeof Ektron == "undefined")
    window.Ektron = {};

	
if(typeof Ektron.Crypto == "undefined")
{	
	Ektron.Crypto = {
	
		SetCryptographicHashAlgorithm : function(cha){
			Ektron.Crypto.SelectedCryptographicHashAlgorithm = cha;
		},
		
		SetEncryptionAlgorithm : function(ea){
			Ektron.Crypto.SelectedEncryptionAlgorithm = ea;
		},
		
		Hash : function(inputValue){
			switch(Ektron.Crypto.SelectedCryptographicHashAlgorithm){
				case Ektron.Crypto.CryptographicHashAlgorithms.MD5:
					return Ektron.Crypto.MD5.hex_md5(inputValue);
					break;
				case Ektron.Crypto.CryptographicHashAlgorithms.SHA1:
					return Ektron.Crypto.SHA1.hex_sha(inputValue);
					break;
				case Ektron.Crypto.CryptographicHashAlgorithms.SHA224:
					return Ektron.Crypto.SHA256.hex_sha(inputValue, "SHA-224");
					break;
				case Ektron.Crypto.CryptographicHashAlgorithms.SHA256:
					return Ektron.Crypto.SHA256.hex_sha(inputValue, "SHA-256");
					break;
				case Ektron.Crypto.CryptographicHashAlgorithms.SHA384:
					return Ektron.Crypto.SHA512.hex_sha(inputValue, "SHA-384");
					break;
				case Ektron.Crypto.CryptographicHashAlgorithms.SHA512:
					return Ektron.Crypto.SHA512.hex_sha(inputValue, "SHA-512");
					break;
				default:
					return "Invalid Hashing Algorithm Selected";
					break;
			}
		},
		
		EncryptData : function(inputValue, key){
			switch(Ektron.Crypto.SelectedEncryptionAlgorithm){
				case Ektron.Crypto.EncryptionAlgorithms.AES128:
					return Ektron.Crypto.AES.Encrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES128)
					break;
				case Ektron.Crypto.EncryptionAlgorithms.AES192:
					return Ektron.Crypto.AES.Encrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES128)
					break;
				case Ektron.Crypto.EncryptionAlgorithms.AES256:
					return Ektron.Crypto.AES.Encrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES256)
					break;
				default:
					return "Invalid Encryption Algorithm Selected";
					break;
			}
		},
		
		DecryptData : function(inputValue, key){
			switch(Ektron.Crypto.SelectedEncryptionAlgorithm){
				case Ektron.Crypto.EncryptionAlgorithms.AES128:
					return Ektron.Crypto.AES.Decrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES128)
					break;
				case Ektron.Crypto.EncryptionAlgorithms.AES192:
					return Ektron.Crypto.AES.Decrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES128)
					break;
				case Ektron.Crypto.EncryptionAlgorithms.AES256:
					return Ektron.Crypto.AES.Decrypt(inputValue, key, Ektron.Crypto.AES.KeyType.AES256)
					break;
				default:
					return "Invalid Encryption Algorithm Selected";
					break;
			}
		}
	};
	
	Ektron.Crypto.CryptographicHashAlgorithms = { MD5 : 1, SHA1 : 2, SHA224 : 3, SHA256 : 4, SHA384 : 5, SHA512: 6 };
	Ektron.Crypto.SelectedCryptographicHashAlgorithm = Ektron.Crypto.CryptographicHashAlgorithms.SHA512;
	
	Ektron.Crypto.EncryptionAlgorithms = { AES128 : 1, AES192 : 2, AES256 : 3 };
	Ektron.Crypto.SelectedEncryptionAlgorithm = Ektron.Crypto.EncryptionAlgorithms.AES256;
}

//Encryption Algorithm(s)

if(typeof Ektron.Crypto.AES == "undefined")
{
	Ektron.Crypto.AES = {

		Encrypt: function(str, key, type)
	    {
			Ektron.Crypto.AES.Nk = type;
			Ektron.Crypto.AES.Nr = type + 6;
		
	        var w = Ektron.Crypto.Convert.StringToByteArray(str);
	        var Nk = key.length / 4;
	        var blockCount = Math.ceil(w.length / 16);
	        
	        var expandedKey = [];
	        Ektron.Crypto.AES.KeyExpansion(key, expandedKey, Nk);
	        
	        var retval = [];
	        
	        Ektron.Crypto.AES.Pad(w, blockCount<<4);
	        for(var i = 0; i < blockCount; i++)
	        {
	            var block = [];
	            Ektron.Crypto.AES.Cipher(w.slice(i << 4, (i << 4) + 16), block, expandedKey);
	            retval = retval.concat(block);
	        }
	        
	        return retval
	    },
	    
	    Decrypt: function(w, key, type)
	    {	        
			Ektron.Crypto.AES.Nk = type;
			Ektron.Crypto.AES.Nr = type + 6;
		
	        var Nk = key.length / 4;
	        var blockCount = w.length / 16;
	        
	        var expandedKey = [];
	        Ektron.Crypto.AES.KeyExpansion(key, expandedKey, Nk);
	        Ektron.PrivateData.Log(expandedKey);
	        
	        var retval = [];
	        for(var i = 0; i < blockCount; i++)
	        {
	            var block = [];
	            Ektron.Crypto.AES.InvCipher(w.slice(i << 4, (i << 4) + 16), block, expandedKey);
	            retval = retval.concat(block);
	        }
	        
	        Ektron.Crypto.AES.UnPad(retval, blockCount<<4);
	        
	        return Ektron.Crypto.Convert.ByteArrayToString(retval);
	    },
	    
		Pad: function(w, blockLength)
	    {
	        w.push(0xFF);
	        var remainder = w.length % blockLength;
	        
	        if(remainder > 0)
	        {
	            var len = blockLength - remainder;
	            while(len-- > 0) w.push(0);
	        }
	    },
	    
	    UnPad: function(w, blockLength)
	    {
	        while(w.pop() != 0xFF && w.length > 0);
	    },

		Cipher: function(input, out, w)
		{
			var state = new Array(new Array(), new Array(), new Array(), new Array());

			for(var i = 0; i < 16; i++)
			{
				state[Math.floor(i/4)][i%4] = input[i];
			}
		
		    if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogRound("Round 0");
			Ektron.Crypto.AES.AddRoundKey(state, w.slice(0,4));
		
			for(var round = 1; round < Ektron.Crypto.AES.Nr; round++)
			{
			    if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogRound("Round " + round);
				Ektron.Crypto.AES.SubBytes(state);
				Ektron.Crypto.AES.ShiftRows(state);
				Ektron.Crypto.AES.MixColumns(state);
				Ektron.Crypto.AES.AddRoundKey(state, w.slice(round*Ektron.Crypto.AES.Nb,round*Ektron.Crypto.AES.Nb+4));
			}
		
		    if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogRound("Round " + Ektron.Crypto.AES.Nr);
			Ektron.Crypto.AES.SubBytes(state);
			Ektron.Crypto.AES.ShiftRows(state);
			Ektron.Crypto.AES.AddRoundKey(state, w.slice(Ektron.Crypto.AES.Nr*Ektron.Crypto.AES.Nb,Ektron.Crypto.AES.Nr*Ektron.Crypto.AES.Nb+4));
		
			for(var i = 0; i < 16; i++)
			{
				out[i] = state[Math.floor(i/4)][i%4];
			}
		},

		InvCipher: function(input, out, w)
		{
			var state = new Array(new Array(), new Array(), new Array(), new Array());

			for(var i = 0; i < 16; i++)
			{
				state[Math.floor(i/4)][i%4] = input[i];
			}

			Ektron.Crypto.AES.AddRoundKey(state, w.slice(Ektron.Crypto.AES.Nr*Ektron.Crypto.AES.Nb, Ektron.Crypto.AES.Nr*Ektron.Crypto.AES.Nb+4));

			for(var round = Ektron.Crypto.AES.Nr - 1; round > 0; round--)
			{
				Ektron.Crypto.AES.InvShiftRows(state);
				Ektron.Crypto.AES.InvSubBytes(state);
				Ektron.Crypto.AES.AddRoundKey(state, w.slice(round*Ektron.Crypto.AES.Nb, round*Ektron.Crypto.AES.Nb+4));
				Ektron.Crypto.AES.InvMixColumns(state);
			}

			Ektron.Crypto.AES.InvShiftRows(state);
			Ektron.Crypto.AES.InvSubBytes(state);
			Ektron.Crypto.AES.AddRoundKey(state, w.slice(0, 4));

			for(var i = 0; i < 16; i++)
			{
				out[i] = state[Math.floor(i/4)][i%4];
			}
		},

		SubBytes: function(state)
		{
			var i, j;
			for(i = 0; i < 4; i++)
				for(j = 0; j < Ektron.Crypto.AES.Nb; j++)
					state[i][j] = Ektron.Crypto.AES.SBOX(state[i][j]);
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("SubBytes", state);
		},

		InvShiftRows: function(state)
		{
			for(var i = 1; i < 4; i++)
			{
				var ps = new Array(state[0][i], state[1][i], state[2][i], state[3][i]);
				for(var j = 0; j < i; j++)
				{
					var tmp = ps[3];
					ps[3] = ps[2];
					ps[2] = ps[1];
					ps[1] = ps[0];
					ps[0] = tmp;
				}
				state[0][i] = ps[0];
				state[1][i] = ps[1];
				state[2][i] = ps[2];
				state[3][i] = ps[3];
			}
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("ShiftRows", state);
		},

		MixColumns: function(state)
		{
			for(var i = 0; i < 4; i++)
			{
				var ps = new Array(state[i][0], state[i][1], state[i][2], state[i][3]);
				var h = ps[0] ^ ps[1] ^ ps[2] ^ ps[3];
				state[i][0] = Ektron.Crypto.AES.XTIME(ps[0])^ps[1]^Ektron.Crypto.AES.XTIME(ps[1])^ps[2]^ps[3];
				state[i][1] = Ektron.Crypto.AES.XTIME(ps[1])^ps[0]^Ektron.Crypto.AES.XTIME(ps[2])^ps[2]^ps[3];
				state[i][2] = Ektron.Crypto.AES.XTIME(ps[2])^ps[0]^Ektron.Crypto.AES.XTIME(ps[3])^ps[1]^ps[3];
				state[i][3] = Ektron.Crypto.AES.XTIME(ps[3])^ps[0]^Ektron.Crypto.AES.XTIME(ps[0])^ps[1]^ps[2];
			}
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("MixColumns", state);
		},

		InvSubBytes: function(state)
		{
			var i, j;
			for(i = 0; i < 4; i++)
				for(j = 0; j < Ektron.Crypto.AES.Nb; j++)
					state[i][j] = Ektron.Crypto.AES.INV_SBOX(state[i][j]);
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("InvSubBytes", state);
		},

		ShiftRows: function(state)
		{
			for(var i = 1; i < 4; i++)
			{
				var ps = new Array(state[0][i], state[1][i], state[2][i], state[3][i]);
				for(var j = 0; j < i; j++)
				{
					var tmp = ps[0];
					ps[0] = ps[1];
					ps[1] = ps[2];
					ps[2] = ps[3];
					ps[3] = tmp;
				}
				state[0][i] = ps[0];
				state[1][i] = ps[1];
				state[2][i] = ps[2];
				state[3][i] = ps[3];
			}
			
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("InvShiftRows", state);
		},

		InvMixColumns: function(state)
		{
			for(var i = 0; i < 4; i++)
			{
				var ps = new Array(state[i][0], state[i][1], state[i][2], state[i][3]);
				state[i][0] = Ektron.Crypto.AES.DOT(0x0e, ps[0]) ^ Ektron.Crypto.AES.DOT(0x0b, ps[1]) ^ Ektron.Crypto.AES.DOT(0x0d, ps[2]) ^ Ektron.Crypto.AES.DOT(0x09, ps[3]);
				state[i][1] = Ektron.Crypto.AES.DOT(0x09, ps[0]) ^ Ektron.Crypto.AES.DOT(0x0e, ps[1]) ^ Ektron.Crypto.AES.DOT(0x0b, ps[2]) ^ Ektron.Crypto.AES.DOT(0x0d, ps[3]);
				state[i][2] = Ektron.Crypto.AES.DOT(0x0d, ps[0]) ^ Ektron.Crypto.AES.DOT(0x09, ps[1]) ^ Ektron.Crypto.AES.DOT(0x0e, ps[2]) ^ Ektron.Crypto.AES.DOT(0x0b, ps[3]);
				state[i][3] = Ektron.Crypto.AES.DOT(0x0b, ps[0]) ^ Ektron.Crypto.AES.DOT(0x0d, ps[1]) ^ Ektron.Crypto.AES.DOT(0x09, ps[2]) ^ Ektron.Crypto.AES.DOT(0x0e, ps[3]);
			}
			
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("InvMixColumns", state);
		},

		AddRoundKey: function(state, roundkey)
		{
			for(var i = 0; i < 4; i++)
				for(var j = 0; j < 4; j++)
					state[i][j] ^= (roundkey[i] & (0xFF << ((3-j)*8))) >>> ((3-j)*8);
			if(Ektron.Crypto.AES.dbg) Ektron.Crypto.AES.LogState("AddRoundKey", state);
		},

		KeyExpansion: function(key, w, Nk)
		{
		    Ektron.PrivateData.Log("Key: " + key + "\nW: " + w + "\nNk: " + Nk);
		
			var temp;
			var i = 0;
		
			while(i < Nk)
			{
				w.push((key[4*i] << 24) + (key[4*i+1] << 16) + (key[4*i+2] << 8) + key[4*i+3]);
				Ektron.PrivateData.Log(" " + (key[4*i] << 24) + " " + (key[4*i+1] << 16) + " " + (key[4*i+2] << 8) + " " + (key[4*i+3]));
				i++;
			}
		
			i = Nk;
			var len = Ektron.Crypto.AES.Nb * (Ektron.Crypto.AES.Nr + 1);
			while(i < len)
			{
				temp = w[i-1];
				if(i % Nk == 0)
				{
					temp = Ektron.Crypto.AES.RotWord(temp);
					temp = Ektron.Crypto.AES.SubWord(temp);
					temp ^= Ektron.Crypto.AES.RCON[i/Nk] << 24;
				}
				else if(Nk > 6 && i % Nk == 4)
				{
					temp = Ektron.Crypto.AES.SubWord(temp);
				}
				w[i] = w[i-Nk] ^ temp;
				i++;
			}
		},

		RotWord: function(w)
		{
			return (w << 8) | (w >>> 24);
		},

		SubWord: function(w)
		{
		    var x = 0;
			for(var i = 0; i < 4; i++)
				x |= Ektron.Crypto.AES.SBOX((w >>> (i * 8)) & 0xFF) << (i * 8);
			return x;
		},
		
		LogState: function(message, state) { }, 
    
		LogRound: function(message) { }, 
		
		SBOX : function(x) { return Ektron.Crypto.AES._SBOX[(x >>> 4) & 0x0f][x & 0x0f]; },
		
		INV_SBOX : function(x) { return Ektron.Crypto.AES._INV_SBOX[(x >>> 4) & 0x0f][x & 0x0f]; },
		
		DOT : function(a, b)
		{
			var result = 0;

			var count = 8;
			while(count--)
			{
				if(b & 1)
					result ^= a;
				if(a & 128)
				{
					a <<= 1;
					a ^= 0x1b;
				}
				else
					a <<= 1;
				b >>>= 1;
			}
			return result;
		},
		
		SHIFT : function(x) { ((x >>> 8) | ((x & 0xFF) << 24)); },
		INVSHIFT : function(x) { return ((x << 8) | ((x & 0xFF000000) >>> 24)); },

		XTIME : function(x) { return ((((x)<<1) ^ (((x)&128) ? 0x01B : 0))&0xFF); }
	};
	
	Ektron.Crypto.AES.Nb = 4;

	Ektron.Crypto.AES.MAX_Nk = 8;
	Ektron.Crypto.AES.MAX_Nr = 14;
	
	Ektron.Crypto.AES._SBOX = new Array(
		new Array(0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76), 
		new Array(0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0), 
		new Array(0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15), 
		new Array(0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75), 
		new Array(0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84), 
		new Array(0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf), 
		new Array(0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8), 
		new Array(0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2), 
		new Array(0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73), 
		new Array(0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb), 
		new Array(0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79), 
		new Array(0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08), 
		new Array(0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a), 
		new Array(0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e), 
		new Array(0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf), 
		new Array(0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16));

	Ektron.Crypto.AES._INV_SBOX = new Array(
		new Array(0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb), 
		new Array(0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb), 
		new Array(0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e), 
		new Array(0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25), 
		new Array(0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92), 
		new Array(0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84), 
		new Array(0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06), 
		new Array(0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b), 
		new Array(0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73), 
		new Array(0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e), 
		new Array(0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b), 
		new Array(0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4), 
		new Array(0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f), 
		new Array(0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef), 
		new Array(0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61), 
		new Array(0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d));
		
	Ektron.Crypto.AES.RCON = new Array(
		0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 
		0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 
		0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 
		0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 
		0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 
		0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 
		0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 
		0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 
		0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 
		0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 
		0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 
		0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 
		0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 
		0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 
		0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 
		0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb);
		
	Ektron.Crypto.AES.KeyType = {AES128: 4, AES192: 6, AES256: 8};
	
	Ektron.Crypto.AES.KeyType.dbg = false;
	Ektron.Crypto.AES.KeyType.Nr = 0;
	Ektron.Crypto.AES.KeyType.Nk = 0;
    
}

//Cryptographic Hashing Algorithm(s)

if(typeof Ektron.Crypto.MD5 == "undefined")
{
	Ektron.Crypto.MD5 = {
		/*
		 * A JavaScript implementation of the RSA Data Security, Inc. MD5 Message
		 * Digest Algorithm, as defined in RFC 1321.
		 * Version 2.1 Copyright (C) Paul Johnston 1999 - 2002.
		 * Other contributors: Greg Holt, Andrew Kepert, Ydnar, Lostinet
		 * Distributed under the BSD License
		 * See http://pajhome.org.uk/crypt/md5 for more info.
		 */

		/*
		 * These are the functions you'll usually want to call
		 * They take string arguments and return either hex or base-64 encoded strings
		 */
		hex_md5 : function(s){ return Ektron.Crypto.MD5.binl2hex(Ektron.Crypto.MD5.core_md5(Ektron.Crypto.MD5.str2binl(s), s.length * Ektron.Crypto.MD5.chrsz));},
		b64_md5 : function(s){ return Ektron.Crypto.MD5.binl2b64(Ektron.Crypto.MD5.core_md5(Ektron.Crypto.MD5.str2binl(s), s.length * Ektron.Crypto.MD5.chrsz));},
		str_md5 : function(s){ return Ektron.Crypto.MD5.binl2str(Ektron.Crypto.MD5.core_md5(Ektron.Crypto.MD5.str2binl(s), s.length * Ektron.Crypto.MD5.chrsz));},
		hex_hmac_md5 : function(key, data) { return Ektron.Crypto.MD5.binl2hex(Ektron.Crypto.MD5.core_hmac_md5(key, data)); },
		b64_hmac_md5 : function(key, data) { return Ektron.Crypto.MD5.binl2b64(Ektron.Crypto.MD5.core_hmac_md5(key, data)); },
		str_hmac_md5 : function(key, data) { return Ektron.Crypto.MD5.binl2str(Ektron.Crypto.MD5.core_hmac_md5(key, data)); },

/*
		 * Perform a simple self-test to see if the VM is working
		 */
		md5_vm_test : function()
		{
			return Ektron.Crypto.MD5.hex_md5("abc") == "900150983cd24fb0d6963f7d28e17f72";
		},

		/*
		 * Calculate the MD5 of an array of little-endian words, and a bit length
		 */
		core_md5 : function(x, len)
		{
			/* append padding */
			x[len >> 5] |= 0x80 << ((len) % 32);
			x[(((len + 64) >>> 9) << 4) + 14] = len;

			var a =  1732584193;
			var b = -271733879;
			var c = -1732584194;
			var d =  271733878;

			for(var i = 0; i < x.length; i += 16)
			{
				var olda = a;
				var oldb = b;
				var oldc = c;
				var oldd = d;

				a = Ektron.Crypto.MD5.md5_ff(a, b, c, d, x[i+ 0], 7 , -680876936);
				d = Ektron.Crypto.MD5.md5_ff(d, a, b, c, x[i+ 1], 12, -389564586);
				c = Ektron.Crypto.MD5.md5_ff(c, d, a, b, x[i+ 2], 17,  606105819);
				b = Ektron.Crypto.MD5.md5_ff(b, c, d, a, x[i+ 3], 22, -1044525330);
				a = Ektron.Crypto.MD5.md5_ff(a, b, c, d, x[i+ 4], 7 , -176418897);
				d = Ektron.Crypto.MD5.md5_ff(d, a, b, c, x[i+ 5], 12,  1200080426);
				c = Ektron.Crypto.MD5.md5_ff(c, d, a, b, x[i+ 6], 17, -1473231341);
				b = Ektron.Crypto.MD5.md5_ff(b, c, d, a, x[i+ 7], 22, -45705983);
				a = Ektron.Crypto.MD5.md5_ff(a, b, c, d, x[i+ 8], 7 ,  1770035416);
				d = Ektron.Crypto.MD5.md5_ff(d, a, b, c, x[i+ 9], 12, -1958414417);
				c = Ektron.Crypto.MD5.md5_ff(c, d, a, b, x[i+10], 17, -42063);
				b = Ektron.Crypto.MD5.md5_ff(b, c, d, a, x[i+11], 22, -1990404162);
				a = Ektron.Crypto.MD5.md5_ff(a, b, c, d, x[i+12], 7 ,  1804603682);
				d = Ektron.Crypto.MD5.md5_ff(d, a, b, c, x[i+13], 12, -40341101);
				c = Ektron.Crypto.MD5.md5_ff(c, d, a, b, x[i+14], 17, -1502002290);
				b = Ektron.Crypto.MD5.md5_ff(b, c, d, a, x[i+15], 22,  1236535329);

				a = Ektron.Crypto.MD5.md5_gg(a, b, c, d, x[i+ 1], 5 , -165796510);
				d = Ektron.Crypto.MD5.md5_gg(d, a, b, c, x[i+ 6], 9 , -1069501632);
				c = Ektron.Crypto.MD5.md5_gg(c, d, a, b, x[i+11], 14,  643717713);
				b = Ektron.Crypto.MD5.md5_gg(b, c, d, a, x[i+ 0], 20, -373897302);
				a = Ektron.Crypto.MD5.md5_gg(a, b, c, d, x[i+ 5], 5 , -701558691);
				d = Ektron.Crypto.MD5.md5_gg(d, a, b, c, x[i+10], 9 ,  38016083);
				c = Ektron.Crypto.MD5.md5_gg(c, d, a, b, x[i+15], 14, -660478335);
				b = Ektron.Crypto.MD5.md5_gg(b, c, d, a, x[i+ 4], 20, -405537848);
				a = Ektron.Crypto.MD5.md5_gg(a, b, c, d, x[i+ 9], 5 ,  568446438);
				d = Ektron.Crypto.MD5.md5_gg(d, a, b, c, x[i+14], 9 , -1019803690);
				c = Ektron.Crypto.MD5.md5_gg(c, d, a, b, x[i+ 3], 14, -187363961);
				b = Ektron.Crypto.MD5.md5_gg(b, c, d, a, x[i+ 8], 20,  1163531501);
				a = Ektron.Crypto.MD5.md5_gg(a, b, c, d, x[i+13], 5 , -1444681467);
				d = Ektron.Crypto.MD5.md5_gg(d, a, b, c, x[i+ 2], 9 , -51403784);
				c = Ektron.Crypto.MD5.md5_gg(c, d, a, b, x[i+ 7], 14,  1735328473);
				b = Ektron.Crypto.MD5.md5_gg(b, c, d, a, x[i+12], 20, -1926607734);

				a = Ektron.Crypto.MD5.md5_hh(a, b, c, d, x[i+ 5], 4 , -378558);
				d = Ektron.Crypto.MD5.md5_hh(d, a, b, c, x[i+ 8], 11, -2022574463);
				c = Ektron.Crypto.MD5.md5_hh(c, d, a, b, x[i+11], 16,  1839030562);
				b = Ektron.Crypto.MD5.md5_hh(b, c, d, a, x[i+14], 23, -35309556);
				a = Ektron.Crypto.MD5.md5_hh(a, b, c, d, x[i+ 1], 4 , -1530992060);
				d = Ektron.Crypto.MD5.md5_hh(d, a, b, c, x[i+ 4], 11,  1272893353);
				c = Ektron.Crypto.MD5.md5_hh(c, d, a, b, x[i+ 7], 16, -155497632);
				b = Ektron.Crypto.MD5.md5_hh(b, c, d, a, x[i+10], 23, -1094730640);
				a = Ektron.Crypto.MD5.md5_hh(a, b, c, d, x[i+13], 4 ,  681279174);
				d = Ektron.Crypto.MD5.md5_hh(d, a, b, c, x[i+ 0], 11, -358537222);
				c = Ektron.Crypto.MD5.md5_hh(c, d, a, b, x[i+ 3], 16, -722521979);
				b = Ektron.Crypto.MD5.md5_hh(b, c, d, a, x[i+ 6], 23,  76029189);
				a = Ektron.Crypto.MD5.md5_hh(a, b, c, d, x[i+ 9], 4 , -640364487);
				d = Ektron.Crypto.MD5.md5_hh(d, a, b, c, x[i+12], 11, -421815835);
				c = Ektron.Crypto.MD5.md5_hh(c, d, a, b, x[i+15], 16,  530742520);
				b = Ektron.Crypto.MD5.md5_hh(b, c, d, a, x[i+ 2], 23, -995338651);

				a = Ektron.Crypto.MD5.md5_ii(a, b, c, d, x[i+ 0], 6 , -198630844);
				d = Ektron.Crypto.MD5.md5_ii(d, a, b, c, x[i+ 7], 10,  1126891415);
				c = Ektron.Crypto.MD5.md5_ii(c, d, a, b, x[i+14], 15, -1416354905);
				b = Ektron.Crypto.MD5.md5_ii(b, c, d, a, x[i+ 5], 21, -57434055);
				a = Ektron.Crypto.MD5.md5_ii(a, b, c, d, x[i+12], 6 ,  1700485571);
				d = Ektron.Crypto.MD5.md5_ii(d, a, b, c, x[i+ 3], 10, -1894986606);
				c = Ektron.Crypto.MD5.md5_ii(c, d, a, b, x[i+10], 15, -1051523);
				b = Ektron.Crypto.MD5.md5_ii(b, c, d, a, x[i+ 1], 21, -2054922799);
				a = Ektron.Crypto.MD5.md5_ii(a, b, c, d, x[i+ 8], 6 ,  1873313359);
				d = Ektron.Crypto.MD5.md5_ii(d, a, b, c, x[i+15], 10, -30611744);
				c = Ektron.Crypto.MD5.md5_ii(c, d, a, b, x[i+ 6], 15, -1560198380);
				b = Ektron.Crypto.MD5.md5_ii(b, c, d, a, x[i+13], 21,  1309151649);
				a = Ektron.Crypto.MD5.md5_ii(a, b, c, d, x[i+ 4], 6 , -145523070);
				d = Ektron.Crypto.MD5.md5_ii(d, a, b, c, x[i+11], 10, -1120210379);
				c = Ektron.Crypto.MD5.md5_ii(c, d, a, b, x[i+ 2], 15,  718787259);
				b = Ektron.Crypto.MD5.md5_ii(b, c, d, a, x[i+ 9], 21, -343485551);

				a = Ektron.Crypto.MD5.safe_add(a, olda);
				b = Ektron.Crypto.MD5.safe_add(b, oldb);
				c = Ektron.Crypto.MD5.safe_add(c, oldc);
				d = Ektron.Crypto.MD5.safe_add(d, oldd);
			}
			return Array(a, b, c, d);
		},

		/*
		 * These functions implement the four basic operations the algorithm uses.
		 */
		md5_cmn : function(q, a, b, x, s, t)
		{
			return Ektron.Crypto.MD5.safe_add(Ektron.Crypto.MD5.bit_rol(Ektron.Crypto.MD5.safe_add(Ektron.Crypto.MD5.safe_add(a, q), Ektron.Crypto.MD5.safe_add(x, t)), s),b);
		},
		md5_ff : function(a, b, c, d, x, s, t)
		{
			return Ektron.Crypto.MD5.md5_cmn((b & c) | ((~b) & d), a, b, x, s, t);
		},
		md5_gg : function(a, b, c, d, x, s, t)
		{
			return Ektron.Crypto.MD5.md5_cmn((b & d) | (c & (~d)), a, b, x, s, t);
		},
		md5_hh : function(a, b, c, d, x, s, t)
		{
			return Ektron.Crypto.MD5.md5_cmn(b ^ c ^ d, a, b, x, s, t);
		},
		md5_ii : function(a, b, c, d, x, s, t)
		{
			return Ektron.Crypto.MD5.md5_cmn(c ^ (b | (~d)), a, b, x, s, t);
		},

		/*
		 * Calculate the HMAC-MD5, of a key and some data
		 */
		core_hmac_md5 : function(key, data)
		{
			var bkey = Ektron.Crypto.MD5.str2binl(key);
			if(bkey.length > 16) bkey = Ektron.Crypto.MD5.core_md5(bkey, key.length * Ektron.Crypto.MD5.chrsz);

			var ipad = Array(16), opad = Array(16);
			for(var i = 0; i < 16; i++)
			{
				ipad[i] = bkey[i] ^ 0x36363636;
				opad[i] = bkey[i] ^ 0x5C5C5C5C;
			}

			var hash = Ektron.Crypto.MD5.core_md5(ipad.concat(Ektron.Crypto.MD5.str2binl(data)), 512 + data.length * Ektron.Crypto.MD5.chrsz);
			return Ektron.Crypto.MD5.core_md5(opad.concat(hash), 512 + 128);
		},

		/*
		 * Add integers, wrapping at 2^32. This uses 16-bit operations internally
		 * to work around bugs in some JS interpreters.
		 */
		safe_add : function(x, y)
		{
			var lsw = (x & 0xFFFF) + (y & 0xFFFF);
			var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
			return (msw << 16) | (lsw & 0xFFFF);
		},

		/*
		 * Bitwise rotate a 32-bit number to the left.
		 */
		bit_rol : function(num, cnt)
		{
			return (num << cnt) | (num >>> (32 - cnt));
		},

		/*
		 * Convert a string to an array of little-endian words
		 * If chrsz is ASCII, characters >255 have their hi-byte silently ignored.
		 */
		str2binl : function(str)
		{
			var bin = Array();
			var mask = (1 << Ektron.Crypto.MD5.chrsz) - 1;
			for(var i = 0; i < str.length * Ektron.Crypto.MD5.chrsz; i += Ektron.Crypto.MD5.chrsz)
				bin[i>>5] |= (str.charCodeAt(i / Ektron.Crypto.MD5.chrsz) & mask) << (i%32);
			return bin;
		},

		/*
		 * Convert an array of little-endian words to a string
		 */
		binl2str : function(bin)
		{
			var str = "";
			var mask = (1 << Ektron.Crypto.MD5.chrsz) - 1;
			for(var i = 0; i < bin.length * 32; i += Ektron.Crypto.MD5.chrsz)
				str += String.fromCharCode((bin[i>>5] >>> (i % 32)) & mask);
			return str;
		},

		/*
		 * Convert an array of little-endian words to a hex string.
		 */
		binl2hex : function(binarray)
		{
			var hex_tab = Ektron.Crypto.MD5.hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
			var str = "";
			for(var i = 0; i < binarray.length * 4; i++)
			{
				str += hex_tab.charAt((binarray[i>>2] >> ((i%4)*8+4)) & 0xF) +
						hex_tab.charAt((binarray[i>>2] >> ((i%4)*8  )) & 0xF);
			}
			return str;
		},

		/*
		 * Convert an array of little-endian words to a base-64 string
		 */
		binl2b64 : function(binarray)
		{
			var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
			var str = "";
			for(var i = 0; i < binarray.length * 4; i += 3)
			{
				var triplet = (((binarray[i   >> 2] >> 8 * ( i   %4)) & 0xFF) << 16)
								| (((binarray[i+1 >> 2] >> 8 * ((i+1)%4)) & 0xFF) << 8 )
								|  ((binarray[i+2 >> 2] >> 8 * ((i+2)%4)) & 0xFF);
				for(var j = 0; j < 4; j++)
				{
					if(i * 8 + j * 6 > binarray.length * 32) str += Ektron.Crypto.MD5.b64pad;
					else str += tab.charAt((triplet >> 6*(3-j)) & 0x3F);
				}
			}
			return str;
		}
	};
	
	/*
	 * Configurable variables. You may need to tweak these to be compatible with
	 * the server-side, but the defaults work in most cases.
	 */
	Ektron.Crypto.MD5.hexcase = 0;  /* hex output format. 0 - lowercase; 1 - uppercase        */
	Ektron.Crypto.MD5.b64pad  = ""; /* base-64 pad character. "=" for strict RFC compliance   */
	Ektron.Crypto.MD5.chrsz   = 8;  /* bits per input character. 8 - ASCII; 16 - Unicode      */
	
}

if(typeof Ektron.Crypto.SHA1 == "undefined")
{

	Ektron.Crypto.SHA1 = {
	
		hex_sha : function(string) { return Ektron.Crypto.SHA1.binb2hex(Ektron.Crypto.SHA1.coreSHA1(Ektron.Crypto.SHA1.str2binb(string), string.length * Ektron.Crypto.SHA1.charSize));},
		
		ROTL_32 : function(x, n)
		{
			if (n < 32)
				return (x <<  n) | (x >>> (32 - n));
			else
				return x
		},

		Parity_32 : function(x, y, z)
		{
			return x ^ y ^ z;
		},

		Ch_32 : function(x, y, z)
		{
			return (x & y) ^ (~x & z);
		},

		Maj_32 : function(x, y, z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		},

		coreSHA1 : function(message, messageLength)
		{
			var W = new Array();
			var a, b, c, d, e;
			var T;
			var Ch = Ektron.Crypto.SHA1.Ch_32, Parity = Ektron.Crypto.SHA1.Parity_32, Maj = Ektron.Crypto.SHA1.Maj_32, ROTL = Ektron.Crypto.SHA1.ROTL_32, safeAdd = Ektron.Crypto.SHA1.safeAdd_32;
			var H = Ektron.Crypto.SHA1.H_1.slice();

			message[messageLength >> 5] |= 0x80 << (24 - messageLength % 32); // Append '1' at  the end of the binary string
			message[((messageLength + 1 + 64 >> 9) << 4) + 15] = messageLength; // Append length of binary string in the position such that the new length is a multiple of 512

			var appendedMessageLength = message.length;

			for (var i = 0; i < appendedMessageLength; i += 16) {
				a = H[0];
				b = H[1];
				c = H[2];
				d = H[3];
				e = H[4];
				for ( var t = 0; t < 80; t++)
				{
					if (t < 16)
						W[t] = message[t + i];
					else
						W[t] = ROTL(W[t-3] ^ W[t-8] ^ W[t-14] ^ W[t-16], 1);
					if (t < 20)
						T = safeAdd(safeAdd(safeAdd(safeAdd(ROTL(a, 5), Ch(b, c, d)), e), Ektron.Crypto.SHA1.K[t]), W[t]);
					else if (t < 40)
						T = safeAdd(safeAdd(safeAdd(safeAdd(ROTL(a, 5), Parity(b, c, d)), e), Ektron.Crypto.SHA1.K[t]), W[t]);
					else if (t < 60)
						T = safeAdd(safeAdd(safeAdd(safeAdd(ROTL(a, 5), Maj(b, c, d)), e), Ektron.Crypto.SHA1.K[t]), W[t]);
					else
						T = safeAdd(safeAdd(safeAdd(safeAdd(ROTL(a, 5), Parity(b, c, d)), e), Ektron.Crypto.SHA1.K[t]), W[t]);
					e = d;
					d = c;
					c = ROTL(b, 30);
					b = a;
					a = T;
				}
				H[0] = safeAdd(a, H[0]);
				H[1] = safeAdd(b, H[1]);
				H[2] = safeAdd(c, H[2]);
				H[3] = safeAdd(d, H[3]);
				H[4] = safeAdd(e, H[4]);
			}
			return H;
		},

		safeAdd_32 : function(x, y)
		{
			var lsw = (x & 0xFFFF) + (y & 0xFFFF);
			var msw = (x >>> 16) + (y >>> 16) + (lsw >>> 16);

			return ((msw & 0xFFFF) << 16) | (lsw & 0xFFFF);
		},

		str2binb : function(str)
		{
			var bin = Array();
			var mask = (1 << Ektron.Crypto.SHA1.charSize) - 1;
			var length = str.length * Ektron.Crypto.SHA1.charSize;;

			for(var i = 0; i < length; i += Ektron.Crypto.SHA1.charSize)
				bin[i>>5] |= (str.charCodeAt(i / Ektron.Crypto.SHA1.charSize) & mask) << (32 - Ektron.Crypto.SHA1.charSize - i%32);

			return bin;
		},

		binb2hex : function(binarray)
		{
			var hex_tab = Ektron.Crypto.SHA1.hexCase ? "0123456789ABCDEF" : "0123456789abcdef";
			var str = "";
			var length = binarray.length * 4;
			for(var i = 0; i < length; i++)
				str += hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8+4)) & 0xF) +
					hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8)) & 0xF);
			return str;
		}
	};

	Ektron.Crypto.SHA1.charSize = 8; /* Number of Bits Per character (8 for ASCII, 16 for Unicode)	  */
	Ektron.Crypto.SHA1.hexCase = 0; /* hex output format. 0 - lowercase; 1 - uppercase		*/

	Ektron.Crypto.SHA1.K = new Array(
		0x5a827999, 0x5a827999, 0x5a827999, 0x5a827999,
		0x5a827999, 0x5a827999, 0x5a827999, 0x5a827999,
		0x5a827999, 0x5a827999, 0x5a827999, 0x5a827999,
		0x5a827999, 0x5a827999, 0x5a827999, 0x5a827999,
		0x5a827999, 0x5a827999, 0x5a827999, 0x5a827999,
		0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1,
		0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1,
		0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1,
		0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1,
		0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1, 0x6ed9eba1,
		0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc,
		0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc,
		0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc,
		0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc,
		0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc, 0x8f1bbcdc,
		0xca62c1d6, 0xca62c1d6, 0xca62c1d6, 0xca62c1d6,
		0xca62c1d6, 0xca62c1d6, 0xca62c1d6, 0xca62c1d6,
		0xca62c1d6, 0xca62c1d6, 0xca62c1d6, 0xca62c1d6,
		0xca62c1d6, 0xca62c1d6, 0xca62c1d6, 0xca62c1d6,
		0xca62c1d6, 0xca62c1d6, 0xca62c1d6, 0xca62c1d6
	);

	Ektron.Crypto.SHA1.H_1 = new Array(
		0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476, 0xc3d2e1f0
	);
}

if(typeof Ektron.Crypto.SHA256 == "undefined")
{
	/* A JavaScript implementation of SHA-224 and SHA-256 hashes, as defined in FIPS PUB 180-2
	 * Version 0.1 Copyright Brian Turek 2008
	 * Distributed under the BSD License
	 * See http://jssha.sourceforge.net/ for more information
	 *
	 * Several functions taken, as noted, from Paul Johnson
	 */
	
	Ektron.Crypto.SHA256 = {
	
		/*
		 * The below function is what you want to call.  It take the string to be hashed, as well as
		 * the SHA-2 variant you want to use (SHA-224, SHA-256)
		 */
		hex_sha : function(string, variant) {return Ektron.Crypto.SHA256.binb2hex(Ektron.Crypto.SHA256.coreSHA2(Ektron.Crypto.SHA256.str2binb(string), string.length * this.charSize, variant));},

		ROTR : function(x, n)
		{
			if (n < 32)
				return (x >>> n) | (x << (32 - n));
			else
				return x
		},

		SHR : function(x, n)
		{
			if (n < 32)
				return x >>> n;
			else
				return 0;
		},

		Ch : function(x, y, z)
		{
			return (x & y) ^ (~x & z);
		},

		Maj : function(x, y, z)
		{
			return (x & y) ^ (x & z) ^ (y & z);
		},

		Sigma0 : function(x)
		{
			return Ektron.Crypto.SHA256.ROTR(x, 2) ^ Ektron.Crypto.SHA256.ROTR(x, 13) ^ Ektron.Crypto.SHA256.ROTR(x, 22);
		},

		Sigma1 : function(x)
		{
			return Ektron.Crypto.SHA256.ROTR(x, 6) ^ Ektron.Crypto.SHA256.ROTR(x, 11) ^ Ektron.Crypto.SHA256.ROTR(x, 25);
		},

		Gamma0 : function(x)
		{
			return Ektron.Crypto.SHA256.ROTR(x, 7) ^ Ektron.Crypto.SHA256.ROTR(x, 18) ^ Ektron.Crypto.SHA256.SHR(x, 3);
		},

		Gamma1 : function(x)
		{
			return Ektron.Crypto.SHA256.ROTR(x, 17) ^ Ektron.Crypto.SHA256.ROTR(x, 19) ^ Ektron.Crypto.SHA256.SHR(x, 10);
		},

		coreSHA2 : function(message, messageLength, variant)
		{
			var W = new Array();
			var a, b, c, d, e, f, g, h;
			var T1, T2;
			var H;
			
			if (variant == "SHA-224")
					H = this.H_224.slice();
			else if (variant == "SHA-256")
					H = this.H_256.slice();
			else
				return "HASH NOT RECOGNIZED";

			message[messageLength >> 5] |= 0x80 << (24 - messageLength % 32); // Append '1' at  the end of the binary string
			message[((messageLength + 1 + 64 >> 9) << 4) + 15] = messageLength; // Append length of binary string in the position such that the new length is a multiple of 1024

			var appendedMessageLength = message.length;

			for (var i = 0; i < appendedMessageLength; i += 16) {
				a = H[0];
				b = H[1];
				c = H[2];
				d = H[3];
				e = H[4];
				f = H[5];
				g = H[6];
				h = H[7];

				for ( var t = 0; t < 64; t++)
				{
					if (t < 16)
						W[t] = message[t + i]; // Bit of a hack - for 32-bit, the second term is ignored
					else
						W[t] = Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.Gamma1(W[t - 2]), W[t - 7]), Ektron.Crypto.SHA256.Gamma0(W[t - 15])), W[t - 16]);

					T1 = Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.safeAdd(h, Ektron.Crypto.SHA256.Sigma1(e)), Ektron.Crypto.SHA256.Ch(e, f, g)), this.K[t]), W[t]);
					T2 = Ektron.Crypto.SHA256.safeAdd(Ektron.Crypto.SHA256.Sigma0(a), Ektron.Crypto.SHA256.Maj(a, b, c));
					h = g;
					g = f;
					f = e;
					e = Ektron.Crypto.SHA256.safeAdd(d, T1);
					d = c;
					c = b;
					b = a;
					a = Ektron.Crypto.SHA256.safeAdd(T1, T2);
				}

				H[0] = Ektron.Crypto.SHA256.safeAdd(a, H[0]);
				H[1] = Ektron.Crypto.SHA256.safeAdd(b, H[1]);
				H[2] = Ektron.Crypto.SHA256.safeAdd(c, H[2]);
				H[3] = Ektron.Crypto.SHA256.safeAdd(d, H[3]);
				H[4] = Ektron.Crypto.SHA256.safeAdd(e, H[4]);
				H[5] = Ektron.Crypto.SHA256.safeAdd(f, H[5]);
				H[6] = Ektron.Crypto.SHA256.safeAdd(g, H[6]);
				H[7] = Ektron.Crypto.SHA256.safeAdd(h, H[7]);
			}

			return Ektron.Crypto.SHA256.returnSHA2(H, variant);
		}, 

		returnSHA2 : function(hashArray, variant)
		{
			switch (variant)
			{
				case "SHA-224":
						return new Array(
								hashArray[0],
								hashArray[1],
								hashArray[2],
								hashArray[3],
								hashArray[4],
								hashArray[5],
								hashArray[6]
							);
					break;
				case "SHA-256":
						return hashArray;
					break;
			}
		},

		/*
		 * Add integers, wrapping at 2^32. This uses 16-bit operations internally
		 * to work around bugs in some JS interpreters.
		 * Taken from Paul Johnson (modified slightly)
		 */
		safeAdd : function(x, y)
		{
			var lsw = (x & 0xFFFF) + (y & 0xFFFF);
			var msw = (x >>> 16) + (y >>> 16) + (lsw >>> 16);

			return ((msw & 0xFFFF) << 16) | (lsw & 0xFFFF);
		},

		/*
		 * Convert a string to an array of big-endian words
		 * If charSize is ASCII, characters >255 have their hi-byte silently ignored.
		 * Taken from Paul Johnson
		 */
		str2binb : function(str)
		{
			var bin = Array();
			var mask = (1 << this.charSize) - 1;
			var length = str.length * this.charSize;;

			for(var i = 0; i < length; i += this.charSize)
				bin[i>>5] |= (str.charCodeAt(i / this.charSize) & mask) << (32 - this.charSize - i%32);

			return bin;
		},

		/*
		 * Convert an array of big-endian words to a hex string.
		 * Taken from Paul Johnson
		 */
		binb2hex : function(binarray)
		{
			var hex_tab = this.hexCase ? "0123456789ABCDEF" : "0123456789abcdef";
			var str = "";
			var length = binarray.length * 4;

			for(var i = 0; i < length; i++)
				str += hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8+4)) & 0xF) +
					hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8)) & 0xF);

			return str;
		}
	};
	
	/*
	 * Configurable variables. Defaults typically work
	 */
	Ektron.Crypto.SHA256.charSize = 8; /* Number of Bits Per character (8 for ASCII, 16 for Unicode)	  */
	Ektron.Crypto.SHA256.hexCase = 0; /* hex output format. 0 - lowercase; 1 - uppercase		*/

	Ektron.Crypto.SHA256.K = new Array(
		0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
		0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
		0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
		0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
		0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
		0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
		0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
		0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
		0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
		0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
		0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
		0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
		0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
		0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
		0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
		0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
	);
	
	Ektron.Crypto.SHA256.H_224 = new Array(
		0xc1059ed8, 0x367cd507, 0x3070dd17, 0xf70e5939,
		0xffc00b31, 0x68581511, 0x64f98fa7, 0xbefa4fa4
	);

	Ektron.Crypto.SHA256.H_256 = new Array(
		0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
		0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
	);
}

if(typeof Ektron.Crypto.SHA512 == "undefined")
{
	Ektron.Crypto.SHA512 = {

		hex_sha : function(string, variant) {return Ektron.Crypto.SHA512.binb2hex(Ektron.Crypto.SHA512.coreSHA2(Ektron.Crypto.SHA512.str2binb(string), string.length * this.charSize, variant));},

		ROTR : function(x, n)
		{
			if (n < 32)
				return new Ektron.Crypto.SHA512.int_64(
						(x.highOrder >>> n) | (x.lowOrder << (32 - n)),
						(x.lowOrder >>> n) | (x.highOrder << (32 - n))
					);
			else if (n == 32) // Apparently in JS, shifting a 32-bit value by 32 yields original value
				return new Ektron.Crypto.SHA512.int_64(x.lowOrder, x.highOrder);
			else
				return Ektron.Crypto.SHA512.ROTR(Ektron.Crypto.SHA512.ROTR(x, 32), n-32);
		},

		SHR : function(x, n)
		{
			if (n < 32)
				return new Ektron.Crypto.SHA512.int_64(
						x.highOrder >>> n,
						x.lowOrder >>> n | (x.highOrder << (32 - n))
					);
			else if (n == 32) // Apparently in JS, shifting a 32-bit value by 32 yields original value
				return new Ektron.Crypto.SHA512.int_64(0, x.highOrder);
			else
				return Ektron.Crypto.SHA512.SHR(Ektron.Crypto.SHA512.SHR(x, 32), n-32);
		},

		Ch : function(x, y, z)
		{
			return new Ektron.Crypto.SHA512.int_64(
					(x.highOrder & y.highOrder) ^ (~x.highOrder & z.highOrder),
					(x.lowOrder & y.lowOrder) ^ (~x.lowOrder & z.lowOrder)
				);
		},

		Maj : function(x, y, z)
		{
			return new Ektron.Crypto.SHA512.int_64(
					(x.highOrder & y.highOrder) ^ (x.highOrder & z.highOrder) ^ (y.highOrder & z.highOrder),
					(x.lowOrder & y.lowOrder) ^ (x.lowOrder & z.lowOrder) ^ (y.lowOrder & z.lowOrder)
				);
		},

		Sigma0 : function(x)
		{
			var ROTR28 = Ektron.Crypto.SHA512.ROTR(x, 28);
			var ROTR34 = Ektron.Crypto.SHA512.ROTR(x, 34);
			var ROTR39 = Ektron.Crypto.SHA512.ROTR(x, 39);

			return new Ektron.Crypto.SHA512.int_64(
					ROTR28.highOrder ^ ROTR34.highOrder ^ ROTR39.highOrder,
					ROTR28.lowOrder ^ ROTR34.lowOrder ^ ROTR39.lowOrder
				);
		},

		Sigma1 : function(x)
		{
			var ROTR14 = Ektron.Crypto.SHA512.ROTR(x, 14);
			var ROTR18 = Ektron.Crypto.SHA512.ROTR(x, 18);
			var ROTR41 = Ektron.Crypto.SHA512.ROTR(x, 41)

			return new Ektron.Crypto.SHA512.int_64(
					ROTR14.highOrder ^ ROTR18.highOrder ^ ROTR41.highOrder,
					ROTR14.lowOrder ^ ROTR18.lowOrder ^ ROTR41.lowOrder
				);
		},

		Gamma0 : function(x)
		{
			var ROTR1 = Ektron.Crypto.SHA512.ROTR(x, 1);
			var ROTR8 = Ektron.Crypto.SHA512.ROTR(x, 8);
			var SHR7 = Ektron.Crypto.SHA512.SHR(x, 7);

			return new Ektron.Crypto.SHA512.int_64(
					ROTR1.highOrder ^ ROTR8.highOrder ^ SHR7.highOrder,
					ROTR1.lowOrder ^ ROTR8.lowOrder ^ SHR7.lowOrder
				);
		},

		Gamma1 : function(x)
		{
			var ROTR19 = Ektron.Crypto.SHA512.ROTR(x, 19);
			var ROTR61 = Ektron.Crypto.SHA512.ROTR(x, 61);
			var SHR6 = Ektron.Crypto.SHA512.SHR(x, 6);

			return new Ektron.Crypto.SHA512.int_64(
					ROTR19.highOrder ^ ROTR61.highOrder ^ SHR6.highOrder,
					ROTR19.lowOrder ^ ROTR61.lowOrder ^ SHR6.lowOrder
				);
		},

		coreSHA2 : function(message, messageLength, variant)
		{
			var W = new Array();
			var a, b, c, d, e, f, g, h;
			var T1, T2;
			var H;

			// Set up the various function handles and variable for the specific variant
			if (variant == "SHA-384")
				H = this.H_384.slice();
			else if (variant == "SHA-512")
				H = this.H_512.slice();
			else
				return "HASH NOT RECOGNIZED";

			message[messageLength >> 5] |= 0x80 << (24 - messageLength % 32); // Append '1' at  the end of the binary string
			message[((messageLength + 1 + 128 >> 10) << 5) + 31] = messageLength; // Append length of binary string in the position such that the new length is a multiple of 1024

			var appendedMessageLength = message.length;

			for (var i = 0; i < appendedMessageLength; i += 32) {
				a = H[0];
				b = H[1];
				c = H[2];
				d = H[3];
				e = H[4];
				f = H[5];
				g = H[6];
				h = H[7];

				for ( var t = 0; t < 80; t++)
				{
					if (t < 16)
						W[t] = new Ektron.Crypto.SHA512.int_64(message[t*2 + i], message[t*2 + i +1]);
					else
						W[t] = Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.Gamma1(W[t - 2]), W[t - 7]), Ektron.Crypto.SHA512.Gamma0(W[t - 15])), W[t - 16]);

					T1 = Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.safeAdd(h, Ektron.Crypto.SHA512.Sigma1(e)), Ektron.Crypto.SHA512.Ch(e, f, g)), this.K[t]), W[t]);
					T2 = Ektron.Crypto.SHA512.safeAdd(Ektron.Crypto.SHA512.Sigma0(a), Ektron.Crypto.SHA512.Maj(a, b, c));
					h = g;
					g = f;
					f = e;
					e = Ektron.Crypto.SHA512.safeAdd(d, T1);
					d = c;
					c = b;
					b = a;
					a = Ektron.Crypto.SHA512.safeAdd(T1, T2);
				}

				H[0] = Ektron.Crypto.SHA512.safeAdd(a, H[0]);
				H[1] = Ektron.Crypto.SHA512.safeAdd(b, H[1]);
				H[2] = Ektron.Crypto.SHA512.safeAdd(c, H[2]);
				H[3] = Ektron.Crypto.SHA512.safeAdd(d, H[3]);
				H[4] = Ektron.Crypto.SHA512.safeAdd(e, H[4]);
				H[5] = Ektron.Crypto.SHA512.safeAdd(f, H[5]);
				H[6] = Ektron.Crypto.SHA512.safeAdd(g, H[6]);
				H[7] = Ektron.Crypto.SHA512.safeAdd(h, H[7]);
			}

			return Ektron.Crypto.SHA512.returnSHA2(H, variant);
		},

		returnSHA2 : function(hashArray, variant)
		{
			switch (variant)
			{
				case "SHA-384":
						return new Array(
								hashArray[0].highOrder, hashArray[0].lowOrder,
								hashArray[1].highOrder, hashArray[1].lowOrder,
								hashArray[2].highOrder, hashArray[2].lowOrder,
								hashArray[3].highOrder, hashArray[3].lowOrder,
								hashArray[4].highOrder, hashArray[4].lowOrder,
								hashArray[5].highOrder, hashArray[5].lowOrder
							);
					break;
				case "SHA-512":
						return new Array(
								hashArray[0].highOrder, hashArray[0].lowOrder,
								hashArray[1].highOrder, hashArray[1].lowOrder,
								hashArray[2].highOrder, hashArray[2].lowOrder,
								hashArray[3].highOrder, hashArray[3].lowOrder,
								hashArray[4].highOrder, hashArray[4].lowOrder,
								hashArray[5].highOrder, hashArray[5].lowOrder,
								hashArray[6].highOrder, hashArray[6].lowOrder,
								hashArray[7].highOrder, hashArray[7].lowOrder
						);
					break;
			}
		},

		safeAdd_32 : function(x, y)
		{
			var lsw = (x & 0xFFFF) + (y & 0xFFFF);
			var msw = (x >>> 16) + (y >>> 16) + (lsw >>> 16);

			return ((msw & 0xFFFF) << 16) | (lsw & 0xFFFF);
		},

		/*
		 * The 64-bit counterpart to safeAdd_32
		 */
		safeAdd : function(x, y) {
			var lsw = (x.lowOrder & 0xFFFF) + (y.lowOrder & 0xFFFF);
			var msw = (x.lowOrder >>> 16) + (y.lowOrder >>> 16) + (lsw >>> 16);
			var lowOrder = ((msw & 0xFFFF) << 16) | (lsw & 0xFFFF)

			lsw = (x.highOrder & 0xFFFF) + (y.highOrder & 0xFFFF) + (msw >>> 16);
			msw = (x.highOrder >>> 16) + (y.highOrder >>> 16) + (lsw >>> 16);
			var highOrder = ((msw & 0xFFFF) << 16) | (lsw & 0xFFFF);

			return new Ektron.Crypto.SHA512.int_64(highOrder, lowOrder);
		},

		/*
		 * Convert a string to an array of big-endian words
		 * If \charSize is ASCII, characters >255 have their hi-byte silently ignored.
		 * Taken from Paul Johnson
		 */
		str2binb : function(str)
		{
			var bin = Array();
			var mask = (1 << this.charSize) - 1;
			var length = str.length * this.charSize;;

			for(var i = 0; i < length; i += this.charSize)
				bin[i>>5] |= (str.charCodeAt(i / this.charSize) & mask) << (32 - this.charSize - i%32);

			return bin;
		},

		/*
		 * Convert an array of big-endian words to a hex string.
		 * Taken from Paul Johnson
		 */
		binb2hex : function(binarray)
		{
			var hex_tab = this.hexCase ? "0123456789ABCDEF" : "0123456789abcdef";
			var str = "";
			var length = binarray.length * 4;

			for(var i = 0; i < length; i++)
				str += hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8+4)) & 0xF) +
					hex_tab.charAt((binarray[i>>2] >> ((3 - i%4)*8)) & 0xF);

			return str;
		},

		/*
		 * Ektron.Crypto.SHA512.int_64 is a object/container for 2 32-bit numbers emulating a 64-bit number
		 */
		int_64 : function(msint_32, lsint_32)
		{
			this.highOrder = msint_32;
			this.lowOrder = lsint_32;
		}
	};
	
	/*
	 * Configurable variables. Defaults typically work
	 */
	Ektron.Crypto.SHA512.charSize = 8; /* Number of Bits Per character (8 for ASCII, 16 for Unicode)	  */
	Ektron.Crypto.SHA512.hexCase = 0; /* hex output format. 0 - lowercase; 1 - uppercase		*/
	
	Ektron.Crypto.SHA512.K = new Array(
		new Ektron.Crypto.SHA512.int_64(0x428a2f98,0xd728ae22), new Ektron.Crypto.SHA512.int_64(0x71374491,0x23ef65cd), new Ektron.Crypto.SHA512.int_64(0xb5c0fbcf,0xec4d3b2f), new Ektron.Crypto.SHA512.int_64(0xe9b5dba5,0x8189dbbc),
		new Ektron.Crypto.SHA512.int_64(0x3956c25b,0xf348b538), new Ektron.Crypto.SHA512.int_64(0x59f111f1,0xb605d019), new Ektron.Crypto.SHA512.int_64(0x923f82a4,0xaf194f9b), new Ektron.Crypto.SHA512.int_64(0xab1c5ed5,0xda6d8118),
		new Ektron.Crypto.SHA512.int_64(0xd807aa98,0xa3030242), new Ektron.Crypto.SHA512.int_64(0x12835b01,0x45706fbe), new Ektron.Crypto.SHA512.int_64(0x243185be,0x4ee4b28c), new Ektron.Crypto.SHA512.int_64(0x550c7dc3,0xd5ffb4e2),
		new Ektron.Crypto.SHA512.int_64(0x72be5d74,0xf27b896f), new Ektron.Crypto.SHA512.int_64(0x80deb1fe,0x3b1696b1), new Ektron.Crypto.SHA512.int_64(0x9bdc06a7,0x25c71235), new Ektron.Crypto.SHA512.int_64(0xc19bf174,0xcf692694),
		new Ektron.Crypto.SHA512.int_64(0xe49b69c1,0x9ef14ad2), new Ektron.Crypto.SHA512.int_64(0xefbe4786,0x384f25e3), new Ektron.Crypto.SHA512.int_64(0x0fc19dc6,0x8b8cd5b5), new Ektron.Crypto.SHA512.int_64(0x240ca1cc,0x77ac9c65),
		new Ektron.Crypto.SHA512.int_64(0x2de92c6f,0x592b0275), new Ektron.Crypto.SHA512.int_64(0x4a7484aa,0x6ea6e483), new Ektron.Crypto.SHA512.int_64(0x5cb0a9dc,0xbd41fbd4), new Ektron.Crypto.SHA512.int_64(0x76f988da,0x831153b5),
		new Ektron.Crypto.SHA512.int_64(0x983e5152,0xee66dfab), new Ektron.Crypto.SHA512.int_64(0xa831c66d,0x2db43210), new Ektron.Crypto.SHA512.int_64(0xb00327c8,0x98fb213f), new Ektron.Crypto.SHA512.int_64(0xbf597fc7,0xbeef0ee4),
		new Ektron.Crypto.SHA512.int_64(0xc6e00bf3,0x3da88fc2), new Ektron.Crypto.SHA512.int_64(0xd5a79147,0x930aa725), new Ektron.Crypto.SHA512.int_64(0x06ca6351,0xe003826f), new Ektron.Crypto.SHA512.int_64(0x14292967,0x0a0e6e70),
		new Ektron.Crypto.SHA512.int_64(0x27b70a85,0x46d22ffc), new Ektron.Crypto.SHA512.int_64(0x2e1b2138,0x5c26c926), new Ektron.Crypto.SHA512.int_64(0x4d2c6dfc,0x5ac42aed), new Ektron.Crypto.SHA512.int_64(0x53380d13,0x9d95b3df),
		new Ektron.Crypto.SHA512.int_64(0x650a7354,0x8baf63de), new Ektron.Crypto.SHA512.int_64(0x766a0abb,0x3c77b2a8), new Ektron.Crypto.SHA512.int_64(0x81c2c92e,0x47edaee6), new Ektron.Crypto.SHA512.int_64(0x92722c85,0x1482353b),
		new Ektron.Crypto.SHA512.int_64(0xa2bfe8a1,0x4cf10364), new Ektron.Crypto.SHA512.int_64(0xa81a664b,0xbc423001), new Ektron.Crypto.SHA512.int_64(0xc24b8b70,0xd0f89791), new Ektron.Crypto.SHA512.int_64(0xc76c51a3,0x0654be30),
		new Ektron.Crypto.SHA512.int_64(0xd192e819,0xd6ef5218), new Ektron.Crypto.SHA512.int_64(0xd6990624,0x5565a910), new Ektron.Crypto.SHA512.int_64(0xf40e3585,0x5771202a), new Ektron.Crypto.SHA512.int_64(0x106aa070,0x32bbd1b8),
		new Ektron.Crypto.SHA512.int_64(0x19a4c116,0xb8d2d0c8), new Ektron.Crypto.SHA512.int_64(0x1e376c08,0x5141ab53), new Ektron.Crypto.SHA512.int_64(0x2748774c,0xdf8eeb99), new Ektron.Crypto.SHA512.int_64(0x34b0bcb5,0xe19b48a8),
		new Ektron.Crypto.SHA512.int_64(0x391c0cb3,0xc5c95a63), new Ektron.Crypto.SHA512.int_64(0x4ed8aa4a,0xe3418acb), new Ektron.Crypto.SHA512.int_64(0x5b9cca4f,0x7763e373), new Ektron.Crypto.SHA512.int_64(0x682e6ff3,0xd6b2b8a3),
		new Ektron.Crypto.SHA512.int_64(0x748f82ee,0x5defb2fc), new Ektron.Crypto.SHA512.int_64(0x78a5636f,0x43172f60), new Ektron.Crypto.SHA512.int_64(0x84c87814,0xa1f0ab72), new Ektron.Crypto.SHA512.int_64(0x8cc70208,0x1a6439ec),
		new Ektron.Crypto.SHA512.int_64(0x90befffa,0x23631e28), new Ektron.Crypto.SHA512.int_64(0xa4506ceb,0xde82bde9), new Ektron.Crypto.SHA512.int_64(0xbef9a3f7,0xb2c67915), new Ektron.Crypto.SHA512.int_64(0xc67178f2,0xe372532b),
		new Ektron.Crypto.SHA512.int_64(0xca273ece,0xea26619c), new Ektron.Crypto.SHA512.int_64(0xd186b8c7,0x21c0c207), new Ektron.Crypto.SHA512.int_64(0xeada7dd6,0xcde0eb1e), new Ektron.Crypto.SHA512.int_64(0xf57d4f7f,0xee6ed178),
		new Ektron.Crypto.SHA512.int_64(0x06f067aa,0x72176fba), new Ektron.Crypto.SHA512.int_64(0x0a637dc5,0xa2c898a6), new Ektron.Crypto.SHA512.int_64(0x113f9804,0xbef90dae), new Ektron.Crypto.SHA512.int_64(0x1b710b35,0x131c471b),
		new Ektron.Crypto.SHA512.int_64(0x28db77f5,0x23047d84), new Ektron.Crypto.SHA512.int_64(0x32caab7b,0x40c72493), new Ektron.Crypto.SHA512.int_64(0x3c9ebe0a,0x15c9bebc), new Ektron.Crypto.SHA512.int_64(0x431d67c4,0x9c100d4c),
		new Ektron.Crypto.SHA512.int_64(0x4cc5d4be,0xcb3e42b6), new Ektron.Crypto.SHA512.int_64(0x597f299c,0xfc657e2a), new Ektron.Crypto.SHA512.int_64(0x5fcb6fab,0x3ad6faec), new Ektron.Crypto.SHA512.int_64(0x6c44198c,0x4a475817)
	);
			
	Ektron.Crypto.SHA512.H_384 = new Array(
		new Ektron.Crypto.SHA512.int_64(0xcbbb9d5d,0xc1059ed8), new Ektron.Crypto.SHA512.int_64(0x629a292a,0x367cd507), new Ektron.Crypto.SHA512.int_64(0x9159015a,0x3070dd17), new Ektron.Crypto.SHA512.int_64(0x152fecd8,0xf70e5939),
		new Ektron.Crypto.SHA512.int_64(0x67332667,0xffc00b31), new Ektron.Crypto.SHA512.int_64(0x98eb44a87,0x68581511), new Ektron.Crypto.SHA512.int_64(0xdb0c2e0d,0x64f98fa7), new Ektron.Crypto.SHA512.int_64(0x47b5481d,0xbefa4fa4)
	);

	Ektron.Crypto.SHA512.H_512 = new Array(
		new Ektron.Crypto.SHA512.int_64(0x6a09e667,0xf3bcc908), new Ektron.Crypto.SHA512.int_64(0xbb67ae85,0x84caa73b), new Ektron.Crypto.SHA512.int_64(0x3c6ef372,0xfe94f82b), new Ektron.Crypto.SHA512.int_64(0xa54ff53a,0x5f1d36f1),
		new Ektron.Crypto.SHA512.int_64(0x510e527f,0xade682d1), new Ektron.Crypto.SHA512.int_64(0x9b05688c,0x2b3e6c1f), new Ektron.Crypto.SHA512.int_64(0x1f83d9ab,0xfb41bd6b), new Ektron.Crypto.SHA512.int_64(0x5be0cd19,0x137e2179)
	);
}

//Encoding

if(typeof Ektron.Crypto.Base64 == "undefined")
{

	Ektron.Crypto.Base64 = {
	    encode : function (input) {
	        var output = "";
	        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
	        var i = 0;
	        while (i < input.length) {
	            chr1 = input[i++];
	            chr2 = input[i++];
	            chr3 = input[i++];
	            enc1 = chr1 >> 2;
	            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
	            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
	            enc4 = chr3 & 63;
	            if (isNaN(chr2)) {
	                enc3 = enc4 = 64;
	            } else if (isNaN(chr3)) {
	                enc4 = 64;
	            }
	            output = output +
	            this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
	            this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);
	        }
	        return output;
	    },

	    decode : function (input) {
	        var output = "";
	        var chr1, chr2, chr3;
	        var enc1, enc2, enc3, enc4;
	        var i = 0;
	        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
	        while (i < input.length) {
	            enc1 = this._keyStr.indexOf(input.charAt(i++));
	            enc2 = this._keyStr.indexOf(input.charAt(i++));
	            enc3 = this._keyStr.indexOf(input.charAt(i++));
	            enc4 = this._keyStr.indexOf(input.charAt(i++));
	            chr1 = (enc1 << 2) | (enc2 >> 4);
	            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
	            chr3 = ((enc3 & 3) << 6) | enc4;
	            output = output + String.fromCharCode(chr1);
	            if (enc3 != 64) {
	                output = output + String.fromCharCode(chr2);
	            }
	            if (enc4 != 64) {
	                output = output + String.fromCharCode(chr3);
	            }
	        }
	        output = Ektron.Crypto.Base64._utf8_decode(output);
	        return output;
	    },
		
		decodeToByteArray : function (input) {
			var out = [];
	        var chr1, chr2, chr3;
	        var enc1, enc2, enc3, enc4;
	        var i = 0;
	        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
	        while (i < input.length) {
	            enc1 = this._keyStr.indexOf(input.charAt(i++));
	            enc2 = this._keyStr.indexOf(input.charAt(i++));
	            enc3 = this._keyStr.indexOf(input.charAt(i++));
	            enc4 = this._keyStr.indexOf(input.charAt(i++));
	            chr1 = (enc1 << 2) | (enc2 >> 4);
	            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
	            chr3 = ((enc3 & 3) << 6) | enc4;
	            out.push(chr1);
				if (enc3 != 64) {
					out.push(chr2);
	            }
	            if (enc4 != 64) {
					out.push(chr3);
	            }
	        }
	        out = Ektron.Crypto.Base64._utf8_decode_byte_array(out);
	        Ektron.PrivateData.Log(out);
	        return out;
	    },

	    _utf8_encode : function (string) {
	        string = string.replace(/\r\n/g,"\n");
	        var utftext = "";
	        for (var n = 0; n < string.length; n++) {
	            var c = string.charCodeAt(n);
	            if (c < 128) {
	                utftext += String.fromCharCode(c);
	            }
	            else if((c > 127) && (c < 2048)) {
	                utftext += String.fromCharCode((c >> 6) | 192);
	                utftext += String.fromCharCode((c & 63) | 128);
	            }
	            else {
	                utftext += String.fromCharCode((c >> 12) | 224);
	                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
	                utftext += String.fromCharCode((c & 63) | 128);
	            }
	        }
	        return utftext;
	    },

	    _utf8_decode : function (utftext) {
	        var string = "";
	        var i = 0;
	        var c = c1 = c2 = 0;
	        while ( i < utftext.length ) {
	            c = utftext.charCodeAt(i);
	            if (c < 128) {
	                string += String.fromCharCode(c);
	                i++;
	            }
	            else if((c > 191) && (c < 224)) {
	                c2 = utftext.charCodeAt(i+1);
	                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
	                i += 2;
	            }
	            else {
	                c2 = utftext.charCodeAt(i+1);
	                c3 = utftext.charCodeAt(i+2);
	                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
	                i += 3;
	            }
	        }
	        return string;
	    },
		
		_utf8_decode_byte_array : function (utfArray) {
	        var outArray = [];
	        var i = 0;
	        var c = c1 = c2 = 0;
	        while ( i < utfArray.length ) {
	            c = utfArray[i];
	            if (c < 128) {
	                outArray.push(c);
	                i++;
	            }
	            else if((c > 191) && (c < 224)) {
	                c2 = utfArray[i+1];
	                outArray.push(((c & 31) << 6) | (c2 & 63));
	                i += 2;
	            }
	            else {
	                c2 = utfArray[i+1];
	                c3 = utfArray[i+2];
	                outArray.push(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
	                i += 3;
	            }
	        }
	        return outArray;
	    }
	};
	
	Ektron.Crypto.Base64._keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

}

if(typeof Ektron.Crypto.Convert == "undefined")
{
	Ektron.Crypto.Convert = {
	    StringToByteArray: function(str)
	    {
	        var out = [];
	        var len = 0;
	        try{
	            len = str.length;
	        }catch(err){
	        
	        }
	        for(var i = 0; i < len; i++)
	        {
	            out.push(str.charCodeAt(i));
	        }
	        return out;
	    },
	    
	    ByteArrayToString: function(w)
	    {
	        var out = "";
	        var len = w.length;
	        for(var i = 0; i < len; i++)
	        {
	            out += String.fromCharCode(w[i]);
	        }
	        return out;
	    }
	};
}