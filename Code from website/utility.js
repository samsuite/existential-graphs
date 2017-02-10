function arrays_equal(a,b) {
	return !(a<b || b<a);
}

function induce_overlay(name,x,y,dx,dy) {
	overlay_div = $('<div id="' + name + '_overlay"> </div>');
	overlay_div.css({'z-index' : 2, 'position' : 'absolute'});
	overlay_div.css('left',x);
	overlay_div.css('top',y);
	$('#paper').parent().append(overlay_div);
	return new Raphael(name+'_overlay',dx,dy);
}

/**
Created by Ricardo Corbucci on 2012-06-13
E-mail: ricardo....@gmail.com
It requires jQuery!
raphaelObj -> receives Raphael.element or Raphael.Paper. Any will do
https://groups.google.com/forum/?fromgroups=#!topic/raphaeljs/ABOC-HCMFK0
*/
function mouse_to_svg_coordinates(raphaelObj,mouseEvent) {
	var paper = raphaelObj.paper ? raphaelObj.paper : raphaelObj;
	var parent = $(paper.canvas).parent();
	var viewBox = paper._viewBox === undefined ? [0, 0, paper.width, paper.height] : paper._viewBox;
	var _x         = 0;
	var _y         = 0;
	var _absoluteX = 0;
	var _absoluteY = 0;
	var _scaleX    = 1;
	var _scaleY    = 1;

	try {
		var element = mouseEvent.target || mouseEvent.srcElement || mouseEvent.originalTarget;

		_scaleX = paper.width  / viewBox[2];
		_scaleY = paper.height / viewBox[3];

		if (element.raphael || element.parentElement.raphael) {
			if (jQuery.browser.chrome)  { _x = mouseEvent.clientX; _y = mouseEvent.clientY; } // Chrome
			if (jQuery.browser.mozilla) { _x = mouseEvent.layerX;  _y = mouseEvent.layerY;  } // Firefox
			if (jQuery.browser.msie)    { _x = mouseEvent.x;       _y = mouseEvent.y;       } // IE
			if (jQuery.browser.opera)   { // Opera
				_x = mouseEvent.pageX - parent.position().left;
				_y = mouseEvent.pageY - parent.position().top;
			}

			_absoluteX = _x + parent.offset().left;
			_absoluteY = _y + parent.offset().top;
			_x = (_x / _scaleX) + viewBox[0];
			_y = (_y / _scaleY) + viewBox[1];
		} else { // Any html object
			_absoluteX = element.offsetLeft + (element.offsetWidth/2);
			_absoluteY = element.offsetTop  + (element.offsetHeight/2);
			_x = (_absoluteX - parent.offset().left) / _scaleX;
			_y = (_absoluteY - parent.offset().top)  / _scaleY;
		}
	} catch (e) {
		console.log(e);
	}

	return {
		x: _x,
		y: _y,
		absoluteX: _absoluteX,
		absoluteY: _absoluteY,
		scaleX: _scaleX,
		scaleY: _scaleY
	};
}

function IsString(s) {
	if(typeof s === 'string' || s instanceof String) {
		return true;
	}
	return false;
}

function DuplicateImmutableDict(dict) {
	return jQuery.extend({},dict);
}

function ReplaceWhitespace(s) {
	if(IsString(s)) {
		return s.replace(/ /g,'_');
	}
}
