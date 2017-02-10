function serializeProofNode(proofnode) {
	var json = {};
	json.mode = proofnode.mode;
	json.ruleName = proofnode.ruleName;
	json.ruleNodes = proofnode.ruleNodes || [];
	json.ruleParams = proofnode.ruleParams || {};
	json.visualParams = proofnode.visualParams || {};
	return json;
}

function serializeProofTree(rootnode,selected) {
	var json = serializeProofNode(rootnode);
	if(rootnode === selected) {
		json.selected = true;
	}
	json.children = [];
	rootnode.next.iterate(function(proofnode) {
		json.children.push(serializeProofTree(proofnode,selected));
	});
	return json;
}

Proof.prototype.SaveProof = function() {
	var json = {};
	json.proofTree = serializeProofTree(this.front, this.current);
	return JSON.stringify(json);
};

function NodeIDArrayToNodeList(nodes, tree) {
	var nodeList = new List();
	for(var i in nodes) {
		var id = nodes[i];
		var node = tree.getChildByIdentifier(id);
		nodeList.push_back(node);
	}
	return nodeList;
}

Proof.prototype.deserializeExecuteTree = function(json, prevnode) {
	var proofnode;
	if(!prevnode) {
		// root construction
		var rule = this.getInferenceRule(json.mode, json.ruleName);
		proofnode = this.applyInferenceRule(
			json.mode, null, null, json.ruleName, rule, new List()
		);
		proofnode.ruleNodes = json.ruleNodes;
		proofnode.constructUI(this.paper);
		proofnode.deconstructUI();
	} else {
		var rule = this.getInferenceRule(json.mode, json.ruleName);
		var ptree = prevnode.nodeTree;
		var pattrs = prevnode.uiAttrs;
		// rebuild nodes
		var nodes = NodeIDArrayToNodeList(json.ruleNodes, ptree);

		// apply rule
		proofnode = this.applyInferenceRule(
			json.mode, ptree, pattrs, json.ruleName, rule, nodes, json.ruleParams,
			json.visualParams
		);
		proofnode.ruleNodes = json.ruleNodes;
	}
	proofnode.prev = prevnode;
	var selectedNode = null;
	for(var i in json.children) {
		var nextNodeJson = json.children[i];
		var nextNodeState = this.deserializeExecuteTree(nextNodeJson, proofnode);
		proofnode.next.push_back(nextNodeState.node);
		if(nextNodeState.selected) {
			selectedNode = nextNodeState.selected;
		}
	}
	if(json.selected) {
		selectedNode = proofnode;
	}
	return {
		node: proofnode,
		selected: selectedNode
	}
};

Proof.prototype.LoadProof = function(jsonString) {
	//D("Loading\n"+jsonString);
	var json = JSON.parse(jsonString);
	if(!json) {
		alert("Could not load proof");
		return;
	}

	this.Reset();
	var treeState = this.deserializeExecuteTree(json.proofTree);
	this.front = treeState.node;
	this.select(treeState.selected);
};
