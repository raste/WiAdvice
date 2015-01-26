/*
	WebKBD: An in-browser keyboard layout switcher for the web
	version 1.2
	
	Copyright (c) 2009 Peter Petrov, http://code.ppetrov.com/webkbd/
	
	Permission is hereby granted, free of charge, to any person
	obtaining a copy of this software and associated documentation
	files (the "Software"), to deal in the Software without
	restriction, including without limitation the rights to use,
	copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the
	Software is furnished to do so, subject to the following
	conditions:
	
	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
	OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
	WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
	OTHER DEALINGS IN THE SOFTWARE.
*/

eval(function(p, a, c, k, e, d) {
e = function(c) {
return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};if(!''.replace(/^/,String)){while(c--){d[e(c)]=k[c]||e(c)}k=[function(e){return d[e]}];e=function(){return'\\w+'};c=1};while(c--){if(k[c]){p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c])}}return p}('3={q:[],x:-1,1N:"1.2",1j:8(){3.F(0);3.k(l,"11",3.R,5);3.k(j,"1O",3.R,5);3.k(j,"B",3.H,5);3.k(j,"J",3.w,5);3.k(j,"L",3.w,5);3.1n()},F:8(d){n a,c,b;4(d===X||d===9){d=3.x+1;1M(d>=3.q.p){d-=3.q.p}}4(N(d)=="T"){z(a=0;a<3.q.p;++a){4(d==3.q[a].U){d=a;1L}}4(N(d)=="T"){d=0}}3.x=d;b=3.q[d];3.1a();z(a=0;a<3.v.p;++a){c=3.v[a];c.1J=b.U;c.1K=b.1P+" [1Q: 1V+1I]";c.17.1c=b.1c;c.17.1R=b.1S}6 7},1W:8(a){t{3.F();3.Z()}u(b){}6 7},v:9,y:9,r:9,s:9,1a:8(){n b,c,a;t{3.v=[];4(j.10){b=j.10("3-12")}m{b=j.1A("a")}z(a=0;a<b.p;++a){c=b.1z(a);4(!3.I(c)){1e}3.v.1B(c)}}u(d){}},Z:8(){4(3.y){t{3.y.11()}u(a){}}},K:8(b,d){4(!(b&&b.A&&b.G)){6 7}n a=b.A,c=b.G("S");4(a){a=a.P()}4(c){c=c.P()}4(a=="1H"){6 5}4((a=="1G")&&(c=="1C"||c=="1E")){6 5}4(d&&(a=="1D"||a=="1U")){6 5}6 7},I:8(a){4(!(a&&a.A&&a.13&&a.G)){6 7}4(a.A.P()!="a"){6 7}4(a.13.2d("3-12")<0){6 7}4(a.G("1X")!="2b://2a.29.2f/3/"){6 7}6 5},1g:8(d,b){n c=2e.2c(b),f,h,a,i;4(!d.18){6 7}t{f=j.O("21")}u(g){t{f=j.O("20")}u(g){t{f=j.O("1Z")}u(g){4(3.K(d,7)){h=d.1Y;a=d.22;i=h+c.p;d.Q=d.Q.15(0,h)+c+d.Q.15(a);d.26(i,i);6 5}6 7}}}4(f.1b){f.1b("B",5,5,9,7,7,7,7,b,b)}m{4(f.Y){f.Y("24",5,5,9,c)}m{4(f.19){f.19("B",5,5,9,7,7,7,7,b,b);f.E=f.C=f.27=b}m{6 7}}}d.18(f);6 5},R:8(a){n b=a.1d||a.W;4(!3.I(b)){3.y=(3.K(b,5)?b:9)}6 5},H:8(e){n f=e.1d||e.W,b,c,d,a;4(e.23||e.28||e.1o){6 5}4(5){b=(N(e.E)=="X")?(e.C):(e.E);d=3.q[3.x];4(d.14[b]){c=d.14[b];a=3.1g(f,c);4(a){6 3.M(e)}e.E=e.C=c}}6 5},w:8(c){n b=c.C,a=7;4(c.S=="J"){4(b==1u||b==1y){3.s=1r(8(){3.s=9},1q)}m{4(b==16){3.r=1r(8(){3.r=9},1q)}}}m{4(c.S=="L"){4(b==1u||b==1y){4(3.s){1v(3.s)}3.s=9;4(c.25||3.r){a=5}}m{4(b==16){4(3.r){1v(3.r)}3.r=9;4(c.1o||3.s){a=5}}}}}4(a){3.F();6 3.M(c)}6 5},1n:8(){n b,a;z(a=0;a<l.1f.p;++a){b=l.1f[a];4(!(b&&b.j&&b.l)){1e}3.k(b.j,"B",3.H,5);3.k(b.j,"J",3.w,5);3.k(b.j,"L",3.w,5)}},o:8(a){3.D(l,"1m",3.o);3.D(l,"1i",3.o);3.D(l,"1x",3.o);3.1j()},V:8(){3.k(l,"1m",3.o);3.k(l,"1i",3.o);3.k(l,"1x",3.o)},k:8(c,b,a,d){4(c.1p){c.1p(b,a,d)}m{4(c.1h){c["e"+b+a]=a;c[b+a]=8(e){6 c["e"+b+a](e||l.2g)};c.1h("1l"+b,c[b+a])}}},D:8(c,b,a,d){4(c.1t){c.1t(b,a,d)}m{4(c.1s){c.1s("1l"+b,c[b+a]);c[b+a]=9;c["e"+b+a]=9}}},M:8(a){a.1F=7;a.1T=5;4(a.1k){a.1k()}4(a.1w){a.1w()}6 7}};3.V();',62,141,'|||webkbd|if|true|return|false|function|null||||||||||document|_addEvent|window|else|var|_onReady|length|layouts|_ksShift|_ksMeta|try|catch|_switchers|_onKeyupdown|layout|_lastFocused|for|tagName|keypress|keyCode|_removeEvent|charCode|switchLayout|getAttribute|_onKeypress|_isSwitcherElement|keydown|_isTextElement|keyup|_cancelEvent|typeof|createEvent|toLowerCase|value|_onFocus|type|string|id|_bindReady|srcElement|undefined|initTextEvent|_restoreFocus|getElementsByClassName|focus|switcher|className|map|substr||style|dispatchEvent|initKeyboardEvent|_findSwitchers|initKeyEvent|color|target|continue|frames|_insertChar|attachEvent|readystatechange|init|stopPropagation|on|DOMContentLoaded|_bindIframes|metaKey|addEventListener|2000|setTimeout|detachEvent|removeEventListener|91|clearTimeout|preventDefault|load|92|item|getElementsByTagName|push|text|iframe|password|returnValue|input|textarea|WinKey|innerHTML|title|break|while|version|activate|name|switch|backgroundColor|bgcolor|cancelBubble|html|Shift|switcherClicked|href|selectionStart|KeyboardEvent|TextEvent|KeyEvents|selectionEnd|altKey|textInput|shiftKey|setSelectionRange|which|ctrlKey|ppetrov|code|http|fromCharCode|indexOf|String|com|event'.split('|'),0,{}))

webkbd.layouts.push({
	id: "EN",
	name: "English",
	//color: "white",
	//bgcolor: "black", /*bgcolor: "blue"*/
	map: {}
});

webkbd.layouts.push({
	id: "BP",
	name: "Bulgarian Phonetic",
	//color: "white",
	//bgcolor: "red",
	map: {65:1040,66:1041,67:1062,68:1044,69:1045,70:1060,71:1043,72:1061,73:1048,74:1049,75:1050,76:1051,77:1052,78:1053,79:1054,80:1055,81:1071,82:1056,83:1057,84:1058,85:1059,86:1046,87:1042,88:1117,89:1066,90:1047,91:1096,92:1102,93:1097,96:1095,97:1072,98:1073,99:1094,100:1076,101:1077,102:1092,103:1075,104:1093,105:1080,106:1081,107:1082,108:1083,109:1084,110:1085,111:1086,112:1087,113:1103,114:1088,115:1089,116:1090,117:1091,118:1078,119:1074,120:1100,121:1098,122:1079,123:1064,124:1070,125:1065,126:1063,224:1072,225:1072,226:1072,228:1072,232:1077,233:1077,234:1077,235:1077,236:1080,237:1080,238:1080,239:1080,242:1086,243:1086,244:1086,246:1086,249:1091,250:1091,251:1091,252:1091,253:1072}
});

webkbd.layouts.push({
	id: "BG",
	name: "Bulgarian BDS",
	//color: "white",
	//bgcolor: "orange",
	map: {34:1063,38:58,39:1095,44:1088,46:1083,47:1073,58:1052,59:1084,60:1056,61:46,62:1051,63:1041,64:63,65:1117,66:1060,67:1066,68:1040,69:1045,70:1054,71:1046,72:1043,73:1057,74:1058,75:1053,76:1042,77:1055,78:1061,79:1044,80:1047,81:1099,82:1048,83:1071,84:1064,85:1050,86:1069,87:1059,88:1049,89:1065,90:1070,91:1094,92:40,93:59,94:61,97:1100,98:1092,99:1098,100:1072,101:1077,102:1086,103:1078,104:1075,105:1089,106:1090,107:1085,108:1074,109:1087,110:1093,111:1076,112:1079,113:44,114:1080,115:1103,116:1096,117:1082,118:1101,119:1091,120:1081,121:1097,122:1102,123:1062,124:41,125:167,224:1100,225:1100,226:1100,228:1100,232:1077,233:1077,234:1077,235:1077,236:1089,237:1089,238:1089,239:1089,242:1076,243:1076,244:1076,246:1076,249:1082,250:1082,251:1082,252:1082,253:1097}
});
