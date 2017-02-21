
/*create table usertypes ([id] int, [description] nvarchar(60))

insert into usertypes (id, description) values (36, 'Guid')
insert into usertypes (id, description) values (56, 'int')
insert into usertypes (id, description) values (61, 'DateTime')
insert into usertypes (id, description) values (104, 'bool')
insert into usertypes (id, description) values (127, 'long')
insert into usertypes (id, description) values (231, 'string')
*/

select 
	'public '+ut.description+' '+c.name+' { get; set; }', c.xusertype
from sysobjects o 
inner join syscolumns c on c.id = o.id
left join usertypes ut on ut.id = c.xusertype
where o.name = 'DeliveryStatusHistory'
and o.xtype = 'U'