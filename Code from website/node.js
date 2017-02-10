////////////////////////////////////////////////////////////////////////////////
// Node for creating trees

function Node(parent) {
	this.subtrees = new List(); // subtree children of node
	this.leaves = new List(); // leaf children of node
	this.id = 0; // id of node
	this.id_gen = 0; // id seed for generating children ids
	this.attributes = {}; // immutable attributes
	this.setParent(parent);
}

Node.prototype.setParent = function(parent) {
	this.parent = parent || null;
	if(!this.parent)
		this.id = 0;
	else
		this.id = this.parent.genChildID();
};

//Gets the level of the node
Node.prototype.getLevel = function () {
	if(!this.parent)
		return 0;
	return this.parent.getLevel() + 1;
};

Node.prototype.getRoot = function() {
	if(!this.parent)
		return this;
	return this.parent.getRoot();
};

Node.prototype.equals = function(onode) {
	// check if two leaves are equal
	if(this.isLeaf() && onode.isLeaf()) {
		if (this.getAttribute("label") &&  onode.getAttribute("label") &&
			this.getAttribute("label") === onode.getAttribute("label")) {
			return true;
		}
	}
	// check if two trees are equal
	else if (!(this.isLeaf() || onode.isLeaf())) {
		if (this.equivalence(onode)) {
			return true;
		}
	}
	return false;
};

Node.prototype.isChild = function (node) {
	if(!this.parent)
		return false;
	if(this.parent == node)
		return true;
	return this.parent.isChild(node);
};

Node.prototype.isLeaf = function () {
    if(this.subtrees.length > 0 || this.leaves.length >0 || !this.parent)
        return false;
	var nid = this.getIdentifier();
	for(var itr = this.parent.leaves.begin(); itr != this.parent.leaves.end(); itr = itr.next) {
		var leaf = itr.val;
		if(leaf.getIdentifier() == nid)
			return true;
	}
    return false;
};

Node.prototype.genChildID = function() {
	return this.id_gen++;
};

Node.prototype.getIdentifier = function() {
	if (this.parent)
		return ""+this.id + "_" + this.parent.getIdentifier();
	else
		return ""+this.id;
};

Node.prototype.addSubtree = function(node) {
	var child;
	if(node) {
		node.setParent(this);
		child = node;
		child.refreshIDs();
	} else
		child = new Node(this);
    this.subtrees.push_back(child);
    return child;
};

Node.prototype.removeSubtree = function(node) {
	var itr = this.subtrees.contains(node);
	if(itr) 
		this.subtrees.erase(itr);
};		
		
Node.prototype.takeNewSubtree = function(node) {
	if(node.parent)
		node.parent.removeSubtree(node);
	return this.addSubtree(node);
};

Node.prototype.addLeaf = function(node) {
    var child;
	if(node) {
		node.setParent(this);
		child = node;
		child.refreshIDs();
	} else
		child= new Node(this);
    this.leaves.push_back(child);
    return child;
};

Node.prototype.removeLeaf = function(node) {
	var itr = this.leaves.contains(node);
	if(itr) 
		this.leaves.erase(itr);
};
		
Node.prototype.takeNewLeaf = function(node) {
	if(node.parent)
		node.parent.removeLeaf(node);			
	return this.addLeaf(node);
};

Node.prototype.removeChild = function(node) {
	if(node.isLeaf()) {
		this.removeLeaf(node);
	} else {
		this.removeSubtree(node);
	}
};

Node.prototype.inOrderFMap = function(func) {
	this.leaves.iterate(function(node) { node.preOrderFMap(func); });
	func(this);
	this.subtrees.iterate(function(node) { node.preOrderFMap(func); });
};

Node.prototype.preOrderFMap = function(func) {
	func(this);
	this.leaves.iterate(function(node) { node.preOrderFMap(func); });
	this.subtrees.iterate(function(node) { node.preOrderFMap(func); });
};

Node.prototype.fmap = function(func) {
	this.preOrderFMap(func);
};

Node.prototype.preOrderFlattenToIDs = function() {
	var ids = [];
	this.preOrderFMap(function(node) { ids.push(node.getIdentifier()); });
	return ids;
};

// generate dict of id -> node
Node.prototype.toDict = function () {
    var dict = {};
	this.fmap(function(node) {
		dict[node.getIdentifier()] = node;
	});
    return dict;
};

Node.prototype.refreshID = function() {
	this.id_gen = 0;
	if(this.parent)
		this.id = this.parent.genChildID();
	else
		this.id = 0;
};

Node.prototype.refreshIDs = function() {
	this.fmap(function(node){ node.refreshID(); });
};


Node.prototype.addAttribute = function(attr, val) {
	this.attributes[attr] = val;
};

Node.prototype.getAttribute = function(attr) {
	return this.attributes[attr];
};

Node.prototype.removeAttribute = function(attr) {
	if(attr in this.attributes)
		delete this.attributes[attr];
};

Node.prototype.getChildByIdentifier = function(id) {
	var aid = null; // branch id list
	if(typeof id === "string") {
		aid = id.split("_");
		aid.reverse();
	} else {
		aid = id; // id array already built
	}
	var id_node;
	if (parseInt(aid[0]) === this.id) { // id filter
		if (aid.length>=2) { // still more ids
			var slice; // slice out rest of ids
			if(aid.length==2) {
				slice = [aid[1]];
			} else {
				slice = aid.slice(1,aid.length);
			}
			// find children
			var itr = this.subtrees.begin();
			while(itr!=this.subtrees.end()) {
				var found_node = itr.val.getChildByIdentifier(slice);
				if (found_node)
					id_node = found_node;
				itr = itr.next;
			}
			itr = this.leaves.begin();
			while(itr!=this.leaves.end()) {
				var found_node = itr.val.getChildByIdentifier(slice);
				if (found_node)
					id_node = found_node;
				itr = itr.next;
			}
		} else {
			return this;
		}
	}
	return id_node;
};

Node.prototype.duplicate = function(parent) {
	var dupNode = new Node();
	dupNode.parent = parent || this.parent;
	dupNode.id = this.id;
	dupNode.id_gen = this.id_gen;
	dupNode.attributes = DuplicateImmutableDict(this.attributes);

	this.subtrees.iterate(function(node){
        var child = node.duplicate(dupNode);
        dupNode.subtrees.push_back(child);
	});
	this.leaves.iterate(function(node){
        var child = node.duplicate(dupNode);
        dupNode.leaves.push_back(child);
	});
    return dupNode;
};
