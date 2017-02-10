////////////////////////////////////////////////////////////////////////////////
// Inference rules are built on a rule validator, applicator, and visual applicator

// creates structs of inference rules based on three main functions of a rule
// valid(tree:Node,nodes:List[Node]) -> bool
// applyRule(tree:Node,nodes:List[Node],[params:{string:any}]) -> {tree: Node, diff: Diff}
// if optional params are input they may be updated in the process of application
// applyVisual(tree:Node,nodes:List[Node],diff:{Diff},attrs:{nodeid:{string:any}},[params:{string:any}]) -> newattrs
function NewInferenceRule(validChecker,ruleApplicator,visualApplicator) {
	return {valid: validChecker, applyRule: ruleApplicator, applyVisual: visualApplicator};
}

// rules:{string:InferenceRule}
// nodes:List[Node]
function ValidateInferenceRules(rules, nodes) {
	var validRules = {};
	if(nodes.length) {
		for(var ruleName in rules) {
			var rule = rules[ruleName];
			if(rule.valid && rule.valid(nodes.first().getRoot(), nodes)) {
				validRules[ruleName] = rule;
			}
		}
	}
	return validRules;
}

// 'inference rule' for reseting to an empty state of just root node and no attributes
function ResetTreeRule() {
	return NewInferenceRule(
		function(tree, nodes) {	return true; }, // filler validator
		function(tree, nodes) { // rule applicator
			var diff = NewDiff();
			if(tree) { // get all ids to delete from reset
				var ids = tree.preOrderFlattenToIDs();
				diff.deletions = ids.slice(1,ids.length);
			}
			var newTree = new Node();
			return {tree: newTree, diff: diff};
		},
		function(tree, nodes, diff, attrs) { // visual applicator
			return {attrs: attrs || {}};
		}
	);
}

// 'inference rule' to represent no tree or visual operations
function NoopTreeRule() {
	return NewInferenceRule(
		function(tree, nodes) {	return true; }, // filler validator
		function(tree, nodes) { // rule applicator
			var diff = NewDiff();
			return {tree: tree, diff: diff};
		},
		function(tree, nodes, diff, attrs) { // visual applicator
			return {attrs: attrs};
		}
	);
}

////////////////////////////////////////////////////////////////////////////////
// Diffs are used in rule applicators to refer to any changes made by the inference rule
// note diff contents should only contain roots of subtrees that have changed

// creates a new diff struct
// diffs are used to mark all elements changed in a rule application
function NewDiff() {
	return {additions: [], deletions: [], changes: []};
}

// changes diff made of nodes into diff made of node ids
function NodeDiffToIdDiff(diff) {
	var idiff = NewDiff();
	var i;
	// additions:[Node] -> [ids]
	for(i in diff.additions) {
		var a = diff.additions[i];
		if(IsString(a)) {
			 // ignore ids
			idiff.additions.push(a);
		} else {
			idiff.additions.push(a.getIdentifier());
		}
	}
	// deletions:[Node] -> [ids]
	for(i in diff.deletions) {
		var d = diff.deletions[i];
		if(IsString(d)) {
			// ignore ids
			idiff.deletions.push(d);
		} else {
			idiff.deletions.push(d.getIdentifier());
		}
	}
	// changes: [[oldid,newNode]] [[oldid,newid]]
	for(i in diff.changes) {
		var c = diff.changes[i];
		if(IsString(c[1])) {
			// ignore new ids
			idiff.changes.push(c);
		} else {
			idiff.changes.push([c[0], c[1].getIdentifier()]);
		}
	}
	return idiff;
}

// update attrs with diff information
function UpdateAttrsWithDiff(attrs, diff) {
	// remove attrs
	DeleteAttrs(diff.deletions, attrs);

	// shift attrs
	ShiftAttrs(diff.changes, attrs);

	// init attrs
	AddAttrs(diff.additions, attrs);

	return attrs;
}
