////////////////////////////////////////////////////////////////////////////////
// Proofnodes used in Proofs, acts as a tree of node and ui trees

function ProofNode(nodeTree) {
	this.nodeTree = nodeTree; // main node of tree
	this.uiTree = null; // ui set used to hold ui nodes when rendering node

	this.next = new List();
	this.prev = null;

	// proof node properties, serilizable
	this.mode =	 null; // mode node is in
	this.uiAttrs = null; // node id -> attr map
	this.ruleName =	 null; // rule name
	this.ruleNodes =  null; // array of node ids
	this.ruleParams = null;
	this.visualParams = null;
}

ProofNode.prototype.constructUI = function(R) {
	this.uiTree = new UINodeTree(R, this.nodeTree, Level, Variable, this.uiAttrs);
	this.updateAttrs();
};

ProofNode.prototype.deconstructUI = function() {
	if(this.uiTree) {
		this.updateAttrs();
		this.uiTree.deconstructUI();
		this.uiTree = null;
	}
};

ProofNode.prototype.updateAttrs = function() {
	if(this.uiTree) {
		this.uiAttrs = this.uiTree.getAttrs();
	}
};

function ProofTreeTrim(node){
	if(node.prev) {
		return node;
	}

	//Delete proof offshoots by ensuring each node in the main path is an only child.
	node.prev.next = new List();
	node.prev.next.push_back(node);

	return ProofTreeTrim(node.prev);
}

////////////////////////////////////////////////////////////////////////////////
// Proof construction

function Proof(R) {
	this.paper = R;

	// ui reactor map
	this.eventReactors = {};	

	// mode of proof
	this.currentMode = Proof.LOGIC_MODES.GOAL_MODE;
	this.previousMode = Proof.LOGIC_MODES.GOAL_MODE;
}

// reset to an intial state
Proof.prototype.Reset = function() {
	if(this.current) {
		this.current.deconstructUI();
		delete this.current;
	}
	if(this.front) {
		delete this.front;
	}

	// mode of proof
	this.currentMode = Proof.LOGIC_MODES.GOAL_MODE;
	this.previousMode = Proof.LOGIC_MODES.GOAL_MODE;
};

// initial node construction
Proof.prototype.Begin = function() {
	this.current = this.executeRule(this.currentMode,
									null,
									null,
									Proof.MODE_BOOKENDS[this.currentMode].start.name,
									Proof.MODE_BOOKENDS[this.currentMode].start.rule,
									new List());
	this.current.constructUI(R);
	this.front = this.current;
	
	this.activateReactor(Proof.EVENTS.ADD_NODE);
	this.activateReactor(Proof.EVENTS.SELECT_NODE);
	this.changeMode(this.currentMode);	
};

////////////////////////////////////////////////////////////////////////////////
// Proof constants

Proof.SetupConstants = function() {

	// main logic modes
	Proof.LOGIC_MODES = {
		GOAL_MODE: "goal",
		PREMISE_MODE: "premise", 
		PROOF_MODE: "proof", 
		INSERTION_MODE: "insertion"
	};

	// proof mode bookend nodes
	Proof.MODE_BOOKENDS = {
		// goal mode bookends
		"goal": {start: {name: 'Proof: Goal Start', 
						 rule: ResetTreeRule()}, 
				 end: {name: 'Proof: Goal Constructed', 
					   rule: NoopTreeRule()}},
		// premise mode bookends
		"premise": {start: {name: 'Proof: Premise Start', 
							rule: ResetTreeRule()}, 
					end: {name: 'Proof: Premise Constructed', 
						  rule: NoopTreeRule()}},
		// proof mode bookends
		"proof": {start: {name: 'Proof: Proof Start', 
						  rule: NoopTreeRule()}, 
				  end: {name: 'Proof: Proof Constructed', 
						rule: NoopTreeRule()}},
		// insertion mode bookends
		"insertion": {start: {name: 'Proof: Insertion Start', 
							  rule: NewInferenceRule(ValidateInsertionStart,
													 ApplyInsertionStart,
													 ApplyVisualInsertionStart)}, 
					  end: {name: 'Proof: Insertion End', 
							rule: NewInferenceRule(ValidateInsertionEnd,
												   ApplyInsertionEnd,
												   ApplyVisualInsertionEnd)}}
	};

	// proof events
	Proof.EVENTS = {
		CHANGE_MODE: 'changeMode',
		ADD_NODE: 'addNode',
		SELECT_NODE: 'select',
		NEXT_NODE: 'next',
		PREVIOUS_NODE: 'prev',
		AUTOMATED_CHECK: 'automated_check'
	};

};

////////////////////////////////////////////////////////////////////////////////
// Proof inference rules

// get Proof inference rules based on mode
Proof.prototype.inferenceRules = function(mode) {
	var rules = {};
	if(mode == Proof.LOGIC_MODES.PROOF_MODE) {
		rules['Proof: Double Cut'] = NewInferenceRule(ValidateDoubleCut,
													  AddDoubleCut,
													  ApplyVisualDoubleCut);
		rules['Proof: Empty Double Cut'] = NewInferenceRule(ValidateEmptyDoubleCut,
															AddEmptyDoubleCut,
															ApplyVisualEmptyDoubleCut);
		rules['Proof: Reverse Double Cut'] = NewInferenceRule(ValidateReverseDoubleCut,
															  AddReverseDoubleCut,
															  ApplyVisualReverseDoubleCut);
		rules['Proof: Erasure'] = NewInferenceRule(ValidateProofErasure,
												   AddErasure,
												   ApplyVisualErasure);
		rules['Proof: Iteration'] = NewInferenceRule(ValidateIteration,
													 AddIteration,
													 ApplyVisualIteration);
		rules['Proof: Deiteration'] = NewInferenceRule(ValidateDeiteration,
													   AddDeiteration,
													   ApplyVisualDeiteration);
		rules[Proof.MODE_BOOKENDS[Proof.LOGIC_MODES.INSERTION_MODE].start.name] =
			Proof.MODE_BOOKENDS[Proof.LOGIC_MODES.INSERTION_MODE].start.rule;
	}
	if(mode == Proof.LOGIC_MODES.GOAL_MODE || mode == Proof.LOGIC_MODES.PREMISE_MODE) {
		rules['Construction: Variable'] = NewInferenceRule(ValidateVariable, 
														   AddVariable, 
														   ApplyVisualAttrs);
		rules['Construction: Cut'] = NewInferenceRule(ValidateCut,
													  AddCut,
													  ApplyVisualCut);
		rules['Construction: Double Cut'] = NewInferenceRule(ValidateDoubleCut,
															 AddDoubleCut,
															 ApplyVisualDoubleCut);
		rules['Construction: Empty Cut'] = NewInferenceRule(ValidateEmptyCut,
															AddEmptyCut,
															ApplyVisualEmptyCut);
		rules['Construction: Empty Double Cut'] = NewInferenceRule(ValidateEmptyDoubleCut,
																   AddEmptyDoubleCut,
																   ApplyVisualEmptyDoubleCut);
		rules['Construction: Reverse Cut'] = NewInferenceRule(ValidateReverseCut,
															  AddReverseCut,
															  ApplyVisualReverseCut);
		rules['Construction: Reverse Double Cut'] = NewInferenceRule(ValidateReverseDoubleCut,
																	 AddReverseDoubleCut,
																	 ApplyVisualReverseDoubleCut);
		rules['Construction: Erasure'] = NewInferenceRule(ValidateConstructionErasure,
														  AddErasure,
														  ApplyVisualErasure);
		rules['Construction: Iteration'] = NewInferenceRule(ValidateIteration,
														    AddIteration,
														    ApplyVisualIteration);
		rules['Construction: Deiteration'] = NewInferenceRule(ValidateDeiteration,
															  AddDeiteration,
															  ApplyVisualDeiteration);
		//rules['Construction: PL Statement'] = NewInferenceRule();
	}
	if(mode == Proof.LOGIC_MODES.INSERTION_MODE) {
		// get all construction rules
		rules = this.inferenceRules(Proof.LOGIC_MODES.GOAL_MODE);
		// lift all validators into insertion validator
		for(var rule in rules) {
			rules[rule].valid = ValidateInsertionRule(rules[rule].valid);
		}
		// special insertion validators
		rules['Construction: Cut'].valid = ValidateNotOnInsertionRule(rules['Construction: Cut'].valid);
		rules['Construction: Double Cut'].valid = ValidateNotOnInsertionRule(rules['Construction: Double Cut'].valid);
		rules['Construction: Reverse Cut'].valid = ValidateInsertionReverseCut;
		rules['Construction: Reverse Double Cut'].valid = ValidateInsertionReverseDoubleCut;
		rules['Construction: Erasure'].valid = ValidateNotOnInsertionRule(rules['Construction: Erasure'].valid);
		rules['Construction: Iteration'].valid = ValidateInsertionIteration;
	}
	return rules;
};

// get rules for current mode
Proof.prototype.currentInferenceRules = function() {
	return this.inferenceRules(this.currentMode);
};

Proof.prototype.isRuleModeBookend = function(ruleName) {
	for(var mode in Proof.MODE_BOOKENDS) {
		var bookends = Proof.MODE_BOOKENDS[mode];
		if(ruleName == bookends.start.name || ruleName == bookends.end.name)
			return mode;
	}
	return false;
};

Proof.prototype.getInferenceRule = function(mode, ruleName) {
	if(ruleName in this.inferenceRules(mode)) {
		var rules = this.inferenceRules(mode);
		return rules[ruleName];
	} else if (this.isRuleModeBookend(ruleName) !== false) {
		var bookends = Proof.MODE_BOOKENDS[mode];
		if(ruleName == bookends.start.name)
			return bookends.start.rule;
		else if(ruleName == bookends.end.name)
			return bookends.end.rule;
	}
	return null;
};

////////////////////////////////////////////////////////////////////////////////
// Proof rule applications

function cloneAttrs(attrs) {
	var cattrs = {};
	for(var id in attrs) {
		cattrs[id] = DuplicateImmutableDict(attrs[id]);
	}
	return cattrs;
}

Proof.prototype.applyInferenceRule = function(mode, currTree, currAttrs, ruleName, rule, currNodes, currRuleParams, currVisualParams) {
	if(!(this.isRuleModeBookend(ruleName) !== false || ruleName in this.inferenceRules(mode))) {
		return null;
	}

	// duplicate input
	var dtree = (currTree) ? currTree.duplicate() : currTree;
	var dattrs = (currAttrs) ? cloneAttrs(currAttrs) : currAttrs;
	var nodes = new List();
	currNodes.iterate(function(node) {
		nodes.push_back(dtree.getChildByIdentifier(node.getIdentifier()));
	});

	// apply rule
	var ruleState = rule.applyRule(dtree, nodes, currRuleParams);
	if(!ruleState || !ruleState.tree || !ruleState.diff)
		return null;
	var tree = ruleState.tree;
	var diff = ruleState.diff;
	// get final rules params
	var ruleParams = {};
	if(ruleState.params)
		ruleParams = ruleState.params;
	else if(currRuleParams)
		ruleParams = currRuleParams;
	// update visual attributes
	var updatedAttrs = UpdateAttrsWithDiff(dattrs, diff);
	// apply visual attributes
	var visualState = rule.applyVisual(tree, nodes, diff, updatedAttrs, currVisualParams);
	if(!(visualState || visualState.attrs))
		return null;	
	// get final visual params
	var visualParams = {};
	if(visualState.params)
		visualParams = visualState.params;
	else if(currVisualParams)
		visualParams = currVisualParams;

	// create proof node
	var node = new ProofNode(tree);
	node.mode = mode;
	node.ruleName = ruleName;
	node.uiAttrs = visualState.attrs;
	node.ruleParams = ruleParams || {};
	node.visualParams = visualParams || {};
	
	return node;
};

function NodeListToNodeIDs(nodes) {
	var ids = [];
	nodes.iterate(function(node) {
		ids.push(node.getIdentifier());
	});
	return ids;
}

// execute an input rule with parameters
// returns a linked set of proof nodes of rule created nodes
Proof.prototype.executeRule = function(mode, currTree, currAttrs, ruleName, rule, nodes, ruleParams, visualParams) {
	// construct proof node 
	var ruleNodes = NodeListToNodeIDs(nodes);
	var node = this.applyInferenceRule(mode, currTree, currAttrs, ruleName, rule, nodes, ruleParams, visualParams);
	if(!node)
		return null;
	node.ruleNodes = ruleNodes;

	// construct chain if falling on a bookend
	if(this.isRuleModeBookend(ruleName) !== false) {
		// exit current mode and fall into next mode
		if(ruleName == Proof.MODE_BOOKENDS[mode].end.name) {
			var nextMode = this.nextMode(mode);
			var nextBookend = Proof.MODE_BOOKENDS[nextMode].start;
			var emptySelectedNodes = new List();
			var bookendStart = this.applyInferenceRule(this.nextMode(mode),
													   node.nodeTree,
													   node.uiAttrs,
													   nextBookend.name,
													   nextBookend.rule,
													   emptySelectedNodes,
													   null,
													   null);
			bookendStart.ruleNodes = [];
			node.next.push_back(bookendStart);
			bookendStart.prev = node;
		} else { // start new mode
			var newMode = this.isRuleModeBookend(ruleName);
			if(ruleName == Proof.MODE_BOOKENDS[newMode].start.name) {
				node.mode = newMode;
			}
		}
	}

	return node;
};

Proof.prototype.useRuleOnStep = function(ruleName, nodes, ruleParams, visualParams, proofNode) {
	var rule = this.getInferenceRule(proofNode.mode, ruleName);
	if(!rule)
		return;

	var tree = proofNode.nodeTree;
	var attrs = proofNode.uiAttrs;
	// execute rule
	var chain = this.executeRule(proofNode.mode, 
								 tree,
								 attrs,
								 ruleName,
								 rule,
								 nodes,
								 ruleParams,
								 visualParams);
	chain.prev = proofNode;

	// Check to make sure the returned node isn't equivalent to
	// a next step
	if(proofNode.next.length > 0) {
		var nextItr = proofNode.next.head;
		for(var index = 0; index < proofNode.next.length; index++) {
			// check equivalence, node.equals checks Nodes
			var nodeA = nextItr.val.nodeTree;
			var nodeB = chain.nodeTree;
			if(nodeA.hardNodeEquivalence(nodeB)) {
				// Returns existing node to display
				var stillBroken = 10;
				return nextItr.val;
			}
			nextItr = nextItr.next;
		}
	}


	// remove any contiguous identical bookends in chain
	var prevRuleName = (this.isRuleModeBookend(proofNode.ruleName)) ? proofNode.ruleName : null;
	var itr = chain;
	while(itr) {
		if(itr.ruleName == prevRuleName) { // contiguous bookend
			if(itr.prev === proofNode) // replace head of chain
				chain = itr.next.first();
			// skip to next node
			var prev = itr.prev;
			itr = (itr.next.length) ? itr.next.first() : null;
			itr.prev = prev;
		} else {
			// get next rulename
			prevRuleName = (this.isRuleModeBookend(itr.ruleName)) ? itr.ruleName : null;
			itr = (itr.next.length) ? itr.next.first() : null;
		}
	}

	// splice in chain
	proofNode.next.push_back(chain);
	while(chain.next.length) {
		chain = chain.next.begin().val;
	}
	return chain; // return end of chain
};

Proof.prototype.useRuleOnCurrentStep = function(ruleName, nodes, ruleParams, visualParams) {
	this.current.updateAttrs(); // cache all current ui changes
	var current = this.useRuleOnStep(ruleName, nodes, ruleParams, visualParams, this.current);
	this.select(current);
	this.activateReactor(Proof.EVENTS.ADD_NODE);
};

Proof.prototype.endCurrentProofMode = function() {
	// ~a proof never ends, it just reaches a conclusion~
	if(this.current.mode !== Proof.LOGIC_MODES.PROOF_MODE) {
		var bookendRule = Proof.MODE_BOOKENDS[this.current.mode].end.name;
		var current = this.useRuleOnStep(bookendRule, new List(), null, null, this.current);
		this.select(current);
	}
};


////////////////////////////////////////////////////////////////////////////////
// Proof node/mode selection

// selects step from proof
Proof.prototype.select = function(node) {
	if(this.current !== node) {
		if(this.current)
			this.current.deconstructUI();
		this.current = node;
		this.current.constructUI(this.paper);
		this.changeMode(this.current.mode);
		if(node.mode !== Proof.LOGIC_MODES.GOAL_MODE && node.mode !== Proof.LOGIC_MODES.PREMISE_MODE)
			this.automated_check(this.current);
		this.activateReactor(Proof.EVENTS.SELECT_NODE);
		// Update Goal button.
		var goalbutton = document.getElementById("goalbutton");
		goalbutton.innerHTML = 'See Goal';
		goalbutton.setAttribute('value', 'goGoal');
		goalbutton.setAttribute('class', 'btn btn-danger navbar-btn');
		if (this.current.mode === Proof.LOGIC_MODES.GOAL_MODE) {
			goalbutton.disabled = true;
			goalbutton.setAttribute('class', 'btn btn-default navbar-btn');
		} else {
			goalbutton.disabled = false;
		}
	}
};

// moves proof to last step
Proof.prototype.prev = function() {
	if(this.current.prev) {
		this.select(this.current.prev);
		// this.activateReactor(Proof.EVENTS.PREVIOUS_NODE);
	}
};

// moves proof to next step
Proof.prototype.next = function () {
	if(this.current.next && this.current.next.length == 1) {
		this.select(this.current.next.begin().val);
		// this.activateReactor(Proof.EVENTS.NEXT_NODE);
	}
};

// checks if the proof has a fork at the current step
Proof.prototype.TickRefresh = function () {
	if(!this.current.prev) {
		$('#backwardtick').attr("disabled","disabled");
	}
	else {
		$('#backwardtick').removeAttr("disabled");
	}
	if(!this.current.next || this.current.next.length != 1) {
		$('#forwardtick').attr("disabled","disabled");
	}
	else if (this.current.next) {
		$('#forwardtick').removeAttr("disabled");
	}
	
};

// next proof mode given current mode
// default is goal mode
Proof.prototype.nextMode = function(mode) {
	if (mode === Proof.LOGIC_MODES.PREMISE_MODE) {
		return Proof.LOGIC_MODES.PROOF_MODE;
	}
	if (mode === Proof.LOGIC_MODES.INSERTION_MODE) {
		return Proof.LOGIC_MODES.PROOF_MODE;
	}
	if (mode === Proof.LOGIC_MODES.GOAL_MODE) {
		return Proof.LOGIC_MODES.PREMISE_MODE;
	}
	if (mode === Proof.LOGIC_MODES.PROOF_MODE) {
		return Proof.LOGIC_MODES.PROOF_MODE;
	}

	return Proof.LOGIC_MODES.GOAL_MODE;
};

// change proof mode to input mode
Proof.prototype.changeMode = function(mode) {
	this.previousMode = this.currentMode;
	if (mode === Proof.LOGIC_MODES.PREMISE_MODE) {
		this.currentMode = Proof.LOGIC_MODES.PREMISE_MODE;
	}
	if (mode === Proof.LOGIC_MODES.PROOF_MODE) {
		this.currentMode = Proof.LOGIC_MODES.PROOF_MODE;
	}
	if (mode === Proof.LOGIC_MODES.INSERTION_MODE) {
		this.currentMode = Proof.LOGIC_MODES.INSERTION_MODE;
	}
	if (mode === Proof.LOGIC_MODES.GOAL_MODE) {
		this.currentMode = Proof.LOGIC_MODES.GOAL_MODE;
	}

	this.activateReactor(Proof.EVENTS.CHANGE_MODE);
};

////////////////////////////////////////////////////////////////////////////////
// Proof reactors

// add reactor to proof event
Proof.prototype.addReactor = function(event, func) {
	if(event in this.eventReactors)
		this.eventReactors[event].push(func);
	else
		this.eventReactors[event] = [func];
};

// remove reactor to proof event
Proof.prototype.removeReactor = function(event) {
	if (event in this.eventReactors)
		delete this.eventReactors[event];
};

// execute callback on proof event
Proof.prototype.activateReactor = function(event) {
	if (event in this.eventReactors) {
		for(var i in this.eventReactors[event]) {
			this.eventReactors[event][i](this);
		}
	}
};



Proof.prototype.automated_check = function(pnode) {
	var gnode = pnode;
	while(gnode.mode !== Proof.LOGIC_MODES.GOAL_MODE) {
		gnode = gnode.prev;
	}
	//gnode.constructUI();
	var eq = gnode.nodeTree.equals(pnode.nodeTree);
	//gnode.plane.compressTree();
	if(eq) {
		$('#goalAlert').html(
			"<div class='alert alert-success alert-dismissable'>" +
				"<button type='button' class='close' data-dismiss='alert'>&times;</button>" +
					"<strong>The Goal has been reached!</strong>" +
				"</button>" +
			"</div>"
		);
	}
	//gnode.deconstructUI();
};

Proof.prototype.UIReset = function() {
	if (this.current.uiTree) {
		var uitree = this.current.uiTree;
		var uinodes = uitree.uinodes;
		for (var i in uinodes) {
			uinodes[i].drag(0,0);
		}
	}
}

Proof.prototype.getVarNames = function (root, currentList) {
	if (root) {
		if (root.ruleName == "Construction: Variable") {
			var var_name = root.ruleParams.variable_name;
			if (jQuery.inArray(var_name, currentList) == -1) {
				currentList.push(var_name);
			}
		}
		if (root.next.length !== 0) {
			for (i in root.next) {
				this.getVarNames(root.next[i].val, currentList);
			}
		}
	}
}

Proof.prototype.var_list = function() {
	var pn = this.front;
	var varList = [];
	this.getVarNames(pn, varList);
	D(varList);
	return varList;
}

