////////////////////////////////////////////////////////////////////////////////
// Various ui helpers on attrs for rules

function NewPoint(x, y) {
	return {x: x, y: y};
}

function PullPointFromParams(params) {
	if(params && 'point' in params && 'x' in params.point && 'y' in params.point) {
		return NewPoint(params.point.x, params.point.y);
	}
	return null;
}

function AddPointToID(point, id, attrs) {
	if(!(id in attrs)) {
		attrs[id] = {};
	}
	attrs[id].x = point.x;
	attrs[id].y = point.y;
	return attrs;
}

function AddAttrs(ids, attrs) {
	for(var i in ids) {
		var id = ids[i];
		if(!(id in attrs)) {
			attrs[id] = {};
		}
	}
}

function DeleteAttrs(ids, attrs) {
	for(var i in ids) {
		var id = ids[i];
		if(id in attrs) {
			delete attrs[id];
		}
	}
}

function ShiftAttrs(ids, attrs) {
	for(var i in ids) {
		var idc = ids[i];

		var prevID = idc[0];
		id = idc[1];

		if(prevID in attrs) {
			var attr = attrs[prevID];
			delete attrs[prevID];
			attrs[id] = attr;
		}
	}
}

function ChildrenCentroidPoint(tree, parent, attrs) {
	var x = 0;
	var y = 0;
	var centroidReduce = function(node) {
		var attr = attrs[node.getIdentifier()];
		if(attr && attr.x && attr.y) {
			x += attr.x;
			y += attr.y;
		}
	};
	parent.subtrees.iterate(centroidReduce);
	parent.leaves.iterate(centroidReduce);
	x /= parent.subtrees.length + parent.leaves.length;
	y /= parent.subtrees.length + parent.leaves.length;
	return NewPoint(x,y);
}
