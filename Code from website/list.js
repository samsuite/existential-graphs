function ListNode(x) {
	this.val = x;
	this.prev = null;
	this.next = null;
}

function List() {
	this.head = null;
	this.tail = null;
	this.length = 0;
	for(var i = 0; i < arguments.length; i++) {
		this.push_back(arguments[i]);
	}
}

List.prototype.push_back = function(x) {
	var push = new ListNode(x);
	if(this.tail === null) {
		this.head = this.tail = push;
	} else {
		this.tail.next = push;
		push.prev = this.tail;
		this.tail = push;
	}
	this.length++;
};

List.prototype.pop_back = function() {
	if(this.tail === null) {
		return null;
	}
	var ret = this.tail.val;
	if(this.tail.prev === null) {
		this.head = null;
	} else {
		this.tail.prev.next = null;
	}
	this.tail = this.tail.prev;
	this.length--;
	return ret;
};

List.prototype.push_front = function(x) {
	var push = new ListNode(x);
	if(this.head === null) {
		this.head = this.tail = push;
	} else {
		this.head.prev = push;
		push.next = this.head;
		this.head = push;
	}
	this.length++;
};

List.prototype.pop_front = function() {
	if(this.head === null) { return null; }
	if(this.head.next === null) {
		this.tail = null;
	} else {
		this.head.next.prev = null;
	}
	var ret = this.head.val;
	this.head = this.head.next;
	this.length--;
	return ret;
};

//returns pointer to next in list
List.prototype.erase = function(p) {
	if(p === null) { return null; }
	if(p.next !== null) { p.next.prev = p.prev; }
	if(p.prev !== null) { p.prev.next = p.next; }
	if(this.head === p) { this.head = p.next; }
	if(this.tail === p) { this.tail = p.prev; }
	this.length--;
	return p.next;
};

//inserts x after p, returns where x was inserted
List.prototype.insert = function(p, x) {
	if(p === null || p.next === null) {
		this.push_back(x);
		return this.tail;
	}
	var add = new ListNode(x);
	p.next.prev = add;
	add.next = p.next;
	p.next = add;
	add.prev = p;
	this.length++;
	return add;
};

List.prototype.contains = function(value) {
	var itr = this.skipUntil(function(x) {
		return (x === value);
	});
	if(itr === this.end()) {
		return false;
	} else {
		return itr;
	}
};

List.prototype.first = function() {
	return this.head.val;
};

List.prototype.last = function() {
	return this.tail.val;
};

List.prototype.begin = function() {
	return this.head;
};

List.prototype.rbegin = function() {
	return this.tail;
};

List.prototype.end = function() {
	return null;
};

List.prototype.append = function(l) {
	if(!this.length) {
		this.head = l.head;
		this.tail = l.tail;
		this.length = l.length;
		return;
	}
	if(l.length) {
		this.tail.next = l.head;
		l.head.prev = this.tail;
		this.tail = l.tail;
		this.length += l.length;
	}
};

//iterate applicator
List.prototype.iterate = function(iterate_func) {
	var itr = this.begin();
	while(itr !== this.end()) {
		iterate_func(itr.val);
		itr = itr.next;
	}
};

//iterate until condtion is true, return location or end
List.prototype.skipUntil = function(cond_func) {
	var itr = this.begin();
	while(itr !== this.end()) {
		if(cond_func(itr.val)) { break; }
		itr = itr.next;
	}
	return itr;
};


//merge sort function, pass the less then function, should presurve pointers to list (though it will change the order)
List.prototype.sort = function(less_then_func) {
	if(this.length === 0)  { return; }

	var merge_queue = new List();
	var itr = this.head;

	//break list for merge sort and do the first step
	while(itr !== this.end()) {
		if(itr.next === this.end()) {
			itr.prev = itr.next = null;
			merge_queue.push_back(itr);
			break;
		}
		var p = itr.next.next;
		if(less_then_func(itr.val, itr.next.val)) {
			merge_queue.push_back(itr);
			itr.prev = null;
			itr.next.next = null;
			itr = p;
		} else {
			itr.prev = itr.next;
			itr.next.next = itr;
			itr.next.prev = null;
			merge_queue.push_back(itr.next);
			itr.next = null;
			itr = p;
		}
	}
	//merge sort the list
	while(merge_queue.length !== 1) {
		var merge_itr = merge_queue.head;
		var temp_queue = new List();
		while(merge_itr !== merge_queue.end()) {
			if(merge_itr.next === merge_queue.end()) {
				temp_queue.push_back(merge_itr.val);
				break;
			}
			var list1_itr = merge_itr.val;
			var list2_itr = merge_itr.next.val;
			var merged_list;
			var merged_list_itr;
			if(less_then_func(list1_itr.val, list2_itr.val)) {
				merged_list = merged_list_itr = list1_itr;
				list1_itr = list1_itr.next;
			} else {
				merged_list = merged_list_itr = list2_itr;
				list2_itr = list2_itr.next;
			}
			while(list1_itr && list2_itr) {
				if(less_then_func(list1_itr.val, list2_itr.val)) {
					merged_list_itr.next = list1_itr;
					list1_itr.prev = merged_list_itr;
					merged_list_itr = list1_itr;
					list1_itr = list1_itr.next;
					merged_list_itr.next = null;
				} else {
					merged_list_itr.next = list2_itr;
					list2_itr.prev = merged_list_itr;
					merged_list_itr = list2_itr;
					list2_itr = list2_itr.next;
					merged_list_itr.next = null;
				}
			}
			if(list1_itr) {
				merged_list_itr.next = list1_itr;
				list1_itr.prev = merged_list_itr;
			} else {
				merged_list_itr.next = list2_itr;
				list2_itr.prev = merged_list_itr;
			}
			temp_queue.push_back(merged_list);
			merge_itr = merge_itr.next.next;
		}
		merge_queue = temp_queue;
	}
	//fix the head and tail of the sorted list
	this.head = merge_queue.head.val;
	for(itr = this.head; itr.next !== this.end(); itr = itr.next) {}
	this.tail = itr;
};
