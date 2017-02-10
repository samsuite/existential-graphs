////////////////////////////////////////////////////////////////////////////////
// Inference rule for adding variables

function ValidateVariable(tree, nodes) {
	if(nodes.length === 1) {
		var node = nodes.first();
		if(node.isLeaf()) {
			return false;
		}
		return true;
	}
	return false;
}

function AddVariable(tree, nodes, params) {
	var diff = NewDiff();

	if(params && params.variable_name && params.variable_name.length) {
		variable_name = params.variable_name;
	} else {
		var input_name = UIGetVariableName();
		if(input_name && input_name.length) {
			variable_name = input_name;
		} else {
			// currently just inserts blank var
			D('This case does not happen, but should. ( if someone cancels )');
		}
	}

	if(!params) {
		params = {};
	}

	params.variable_name = variable_name;

	var parent = nodes.first();
	var child = parent.addLeaf();
	child.addAttribute('label',variable_name);
	diff.additions.push(child);

	return {tree: tree, diff: NodeDiffToIdDiff(diff), params: params};
}

function ApplyVisualAttrs(tree, nodes, diff, attrs, params) {
	var point = PullPointFromParams(params);
	if(!point) {
		return null;
	}

	var variableID = diff.additions[0];
	AddPointToID(point, variableID, attrs);

	var variable = tree.getChildByIdentifier(variableID);
	var label = variable.getAttribute('label');
	var attr = attrs[variableID];
	attr.text = label;
	return {attrs: attrs};
}

function UIGetVariableName() {

	// blocking prompt
	var answer = window.prompt('Enter a name for your variable', 'Unnamed');

	// get variable
	if (answer === null) {
		variable_name = 'Unnamed';
	} else {
		variable_name = answer.replace(/^\s+|\s+$/g,'');
	}

	// clear whitespace
	variable_name = ReplaceWhitespace(variable_name);
	return variable_name;

//	bootbox.dialog({
//		title: 'Add Variable',
//		message: 'New Variable or Existing Variable?',
//		buttons: {
//			'New': function() {
//				bootbox.prompt('Enter new variable name', function(e) {
//					if (e === null) {
//						variable_name = "UNKNOWN VARIABLE";
//					}
//					else {
//						variable_name = "" + e;
//						variable_name = variable_name.replace(/^\s+|\s+$/g,"");
//					}
//
//				});
//
//			},
//			'Existing': function() {
//				var list = TheProof.var_list();
//				for (i in list) {
//					D(list[i]);
//				}
//
//			},
//			'Close': function() {}
//		}
//	});
//	D(ReplaceWhitespace(variable_name));
//	return ReplaceWhitespace(variable_name);
}
